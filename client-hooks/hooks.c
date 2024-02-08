#include "client-hooks.h"
#include "../minhook/include/MinHook.h"

#include <Winsock2.h>
#include <stdlib.h>

#define ACC_DISCOVERY_PORT 8999

#define NAMED_PIPE_NAME L"\\\\.\\pipe\\acc-connector-pipe"
#define MAX_SERVER_ENTRIES 100
#define MAX_SERVER_NAME_LEN 64
#define MAX_SERVER_NAME_LEN_BYTES (MAX_SERVER_NAME_LEN * 4)

#pragma pack(push, 1)
struct server_entry {
    UINT8 name[MAX_SERVER_NAME_LEN_BYTES];
    UINT8 name_len;
    UINT32 ip;
    UINT16 port;
};

struct shared_memory {
    struct server_entry servers[MAX_SERVER_ENTRIES];
};
#pragma pack(pop)

int (*real_sendto)(SOCKET, const char*, int, int, const struct sockaddr*, int);
int (*real_recvfrom)(SOCKET, char*, int, int, struct sockaddr*, int*);

SOCKET discoverySocket = -1;
int discoveryId = -1;
int discoveryServerIdx = -1;

struct shared_memory shm;

void handle_discovery(SOCKET s, int id) {
    discoverySocket = s;
    discoveryId = id;
    discoveryServerIdx = 0;

    memset(&shm, 0, sizeof(shm));

    HANDLE hPipe = CreateFileW(NAMED_PIPE_NAME, FILE_GENERIC_READ | FILE_GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
    if (hPipe == INVALID_HANDLE_VALUE) {
        log_msg(L"CreateFileW(\"%s\") failed: 0x%x", NAMED_PIPE_NAME, GetLastError());
        return;
    }
    log_msg(L"CreateFileW done");

    DWORD mode = PIPE_READMODE_MESSAGE;
    if (!SetNamedPipeHandleState(hPipe, &mode, NULL, NULL)) {
        log_msg(L"SetNamedPipeHandleState(\"%s\") failed: 0x%x", NAMED_PIPE_NAME, GetLastError());
        goto cleanup;
    }

    DWORD bytesRead;
    if (!ReadFile(hPipe, &shm, sizeof(struct shared_memory), &bytesRead, NULL)) {
        log_msg(L"ReadFile(\"%s\") failed: 0x%x", NAMED_PIPE_NAME, GetLastError());
        goto cleanup;
    }

    log_msg(L"Read %d bytes from pipe", bytesRead);

cleanup:
    CloseHandle(hPipe);
}

int my_sendto(SOCKET s, const unsigned char* buf, int len, int flags, const struct sockaddr* to, int tolen) {
    if (to->sa_family == AF_INET && tolen >= sizeof(struct sockaddr_in)) {
        struct sockaddr_in* sin = (struct sockaddr_in*)to;
        if (sin->sin_port == _byteswap_ushort(ACC_DISCOVERY_PORT) && len == 6 && buf[0] == 0xbf && buf[1] == 0x48) {
            log_msg(L"Discovery packet detected with id %d", discoveryId);
            handle_discovery(s, buf[2]);
        }
    }
    return real_sendto(s, buf, len, flags, to, tolen);
}

int build_packet(struct server_entry* server, UINT8* buf, int len) {
    size_t needed = (size_t)server->name_len * 4 + 11;
    if (len < needed) {
        return -1;
    }

    int off = 0;

    buf[off++] = 0xc0;

    buf[off++] = server->name_len;
    for (int i = 0; i < server->name_len * 4; i++) {
        buf[off++] = server->name[i];
    }

    buf[off++] = 0x00;
    buf[off++] = 0x01;

    buf[off++] = (server->port >> 8) & 0xff;
    buf[off++] = server->port & 0xff;
    buf[off++] = discoveryId & 0xff;

    buf[off++] = 0x00;
    buf[off++] = 0x00;
    buf[off++] = 0x00;
    buf[off++] = 0xfa;

    return off;
}

int my_recvfrom(SOCKET s, char* buf, int len, int flags, struct sockaddr* from, int* fromlen) {
	if (s == discoverySocket &&
		discoveryServerIdx < MAX_SERVER_ENTRIES &&
		shm.servers[discoveryServerIdx].ip != 0 &&
		*fromlen >= sizeof(struct sockaddr_in)) {

        log_msg(L"Building response packet for index %d", discoveryServerIdx);
		struct server_entry* server = &shm.servers[discoveryServerIdx++];
		struct sockaddr_in* sin = (struct sockaddr_in *)from;
		sin->sin_family = AF_INET;
		sin->sin_port = _byteswap_ushort(ACC_DISCOVERY_PORT);
		sin->sin_addr.S_un.S_addr = server->ip;
		int n = build_packet(server, buf, len);
		if (n != -1) {
			return n;
		}
	}

    return real_recvfrom(s, buf, len, flags, from, fromlen);
}

BOOL attachHooks() {
    MH_STATUS res;
    res = MH_Initialize();
    if (res != MH_OK) {
        log_msg(L"Failed to initialize MinHook, error %s", MH_StatusToString(res));
        return 0;
    }

#define CREATE_HOOK(lib, func, detour, orig)                                                  \
    do {                                                                                      \
        res = MH_CreateHookApi(lib, func, detour, (LPVOID*)orig);                             \
        if (res != MH_OK) {                                                                   \
            log_msg(L"ERROR: Failed to hook %s/%S, error %s", lib, func, MH_StatusToString(res)); \
            return FALSE;                                                                     \
        }                                                                                     \
    } while (0)
    CREATE_HOOK(L"ws2_32.dll", "sendto", my_sendto, &real_sendto);
    CREATE_HOOK(L"ws2_32.dll", "recvfrom", my_recvfrom, &real_recvfrom);

    res = MH_EnableHook(MH_ALL_HOOKS);
    if (res != MH_OK) {
        log_msg(L"Failed to enable hooks, error %s", MH_StatusToString(res));
        return FALSE;
    }

    return TRUE;
}

void removeHooks() {
    MH_DisableHook(MH_ALL_HOOKS);
    MH_Uninitialize();
}
