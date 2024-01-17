#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <Winsock2.h>

#include <stdlib.h>
#include <stdio.h>

#include "../minhook/include/MinHook.h"

#define ACC_DISCOVERY_PORT 8999

#define SHARED_MEMORY_NAME L"acc-connector-shm"
#define MAX_SERVER_ENTRIES 200

#pragma pack(push, 1)
struct server_entry {
    char name[64];
    UINT32 ip;
    UINT16 port;
};

struct shared_memory {
    struct server_entry servers[MAX_SERVER_ENTRIES];
};
#pragma pack(pop)

void log(const wchar_t* fmt, ...) {
    va_list args;
    wchar_t buffer[512];
    va_start(args, fmt);
    vswprintf_s(buffer, 512, fmt, args);
    va_end(args);
    OutputDebugStringW(buffer);
}

int (*real_sendto)(SOCKET, const char*, int, int, const struct sockaddr*, int);
int (*real_recvfrom)(SOCKET, char*, int, int, struct sockaddr*, int*);

SOCKET discoverySocket = -1;
int discoveryId = -1;
int discoveryServerIdx = -1;

HANDLE shmHandle = NULL;
struct shared_memory* shm = NULL;

void resetDiscovery(SOCKET s, int id) {
    discoverySocket = s;
    discoveryId = id;
    discoveryServerIdx = 0;
}

int my_sendto(SOCKET s, const char* buf, int len, int flags, const struct sockaddr* to, int tolen) {
    if (to->sa_family == AF_INET && tolen >= sizeof(struct sockaddr_in)) {
        struct sockaddr_in* sin = (struct sockaddr_in*)to;
        if (sin->sin_addr.S_un.S_addr == INADDR_BROADCAST && sin->sin_port == _byteswap_ushort(ACC_DISCOVERY_PORT) && len == 6) {
            resetDiscovery(s, buf[2]);
            log(L"Discovery packet detected with id %d", discoveryId);
        }
    }
    return real_sendto(s, buf, len, flags, to, tolen);
}

int build_packet(struct server_entry* server, UINT8* buf, int len) {
    size_t needed = strlen(server->name) * 4 + 11;
    if (len < needed) {
        return -1;
    }

    int off = 0;

    buf[off++] = 0xc0;
    buf[off++] = (UINT8)strlen(server->name);
    for (int i = 0; server->name[i] != '\0'; i++) {
        buf[off++] = server->name[i];
        buf[off++] = 0x00;
        buf[off++] = 0x00;
        buf[off++] = 0x00;
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
    if (s == discoverySocket && discoveryId != -1) {
        if (discoveryServerIdx >= MAX_SERVER_ENTRIES || shm->servers[discoveryServerIdx].ip == 0) {
            resetDiscovery(-1, -1);
        }
        else if (*fromlen >= sizeof(struct sockaddr_in)) {
            log(L"Building response packet for index %d", discoveryServerIdx);
            struct server_entry* server = &shm->servers[discoveryServerIdx++];
            struct sockaddr_in* sin = from;
            sin->sin_family = AF_INET;
            sin->sin_port = _byteswap_ushort(ACC_DISCOVERY_PORT);
            sin->sin_addr.S_un.S_addr = server->ip;
            int n = build_packet(server, buf, len);
            if (n != -1) {
                return n;
            }
        }
    }
    return real_recvfrom(s, buf, len, flags, from, fromlen);
}

BOOL attachHooks() {
    MH_STATUS res;

    res = MH_Initialize();
    if (res != MH_OK) {
        log(L"Failed to initialize MinHook, error %s", MH_StatusToString(res));
        return 0;
    }

#define CREATE_HOOK(lib, func, detour, orig)                                                  \
    do {                                                                                      \
        res = MH_CreateHookApi(lib, func, detour, (LPVOID*)orig);                             \
        if (res != MH_OK) {                                                                   \
            log(L"ERROR: Failed to hook %s/%S, error %s", lib, func, MH_StatusToString(res)); \
            return FALSE;                                                                     \
        }                                                                                     \
    } while (0)
    CREATE_HOOK(L"ws2_32.dll", "sendto", my_sendto, &real_sendto);
    CREATE_HOOK(L"ws2_32.dll", "recvfrom", my_recvfrom, &real_recvfrom);

    res = MH_EnableHook(MH_ALL_HOOKS);
    if (res != MH_OK) {
        log(L"Failed to enable hooks, error %s", MH_StatusToString(res));
        return FALSE;
    }

    return TRUE;
}

BOOL openSharedMemory() {
    shmHandle = OpenFileMappingW(PAGE_READONLY, FALSE, SHARED_MEMORY_NAME);
    if (shmHandle == NULL) {
        log(L"OpenFileMappingW failed");
        return FALSE;
    }

    shm = (struct shared_memory*)MapViewOfFile(shmHandle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(struct shared_memory));
    if (shm == NULL) {
        log(L"MapViewOfFile failed");
        return FALSE;
    }

    return TRUE;
}

void closeSharedMemory() {
    if (shm != NULL) {
        UnmapViewOfFile(shm);
    }
    if (shmHandle != NULL) {
        CloseHandle(shmHandle);
    }
}

void removeHooks() {
    MH_DisableHook(MH_ALL_HOOKS);
    MH_Uninitialize();
}

HANDLE watcherMutexHandle = NULL;
HINSTANCE dllInstance;
HANDLE watcherThreadHandle;

DWORD watcher(LPVOID unused) {
	DWORD res = WaitForSingleObject(watcherMutexHandle, INFINITE);
	log(L"Acquired mutex with status %d", res);
    FreeLibraryAndExitThread(dllInstance, 0);
}

BOOL startWatcher() {
    watcherMutexHandle = OpenMutexW(SYNCHRONIZE, FALSE, L"acc-connector-event");
    if (watcherMutexHandle == INVALID_HANDLE_VALUE) {
        log(L"Failed to open event handle");
        return FALSE;
    }

    watcherThreadHandle = CreateThread(NULL, 0, watcher, NULL, 0, NULL);
	if (watcherThreadHandle == NULL) {
		log(L"CreateThread failed");
        return FALSE;
	}

    return TRUE;
}

void stopWatcher() {
    if (watcherMutexHandle != NULL) {
        CloseHandle(watcherMutexHandle);
    }
}

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD fdwReason, LPVOID lpvReserved)
{
    switch (fdwReason)
    {
    case DLL_PROCESS_ATTACH:
        dllInstance = hInstance;
        log(L"DLL attaching...");
        if (!openSharedMemory()) {
            log(L"Failed to open shared memory");
            return FALSE;
        }
        if (!attachHooks()) {
            log(L"Failed to attach hooks");
            return FALSE;
        }
        if (!startWatcher()) {
            log(L"Failed to start watcher");
            return FALSE;
        }
        break;

    case DLL_PROCESS_DETACH:
        log(L"DLL detaching...");
        stopWatcher();
        removeHooks();
        closeSharedMemory();
        break;

    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;
    }

    return TRUE;  // Successful DLL_PROCESS_ATTACH.
}
