#include <string>
#include <sstream>
#include <ostream>
#include <list>

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <Winsock2.h>
#include <WS2tcpip.h>

#pragma comment(lib, "Ws2_32.lib")

#include "resource.h"

std::wstring hookDLLPath(L"C:\\Users\\lonew\\source\\repos\\acc-connector\\x64\\Debug\\client-hooks.dll");

extern "C" {
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
}

class debugstreambuf : public std::basic_stringbuf<wchar_t, std::char_traits<wchar_t>> {
public:
	debugstreambuf(void (*outputFunc)(const wchar_t *)) : m_outputFunc(outputFunc) {
	}

	virtual ~debugstreambuf() {
		sync();
	}

protected:
	int sync() {
		m_outputFunc(str().c_str());
		str(std::wstring());
		return 0;
	}

private:
	void (*m_outputFunc)(const wchar_t *);
};

class basic_dostream : public std::basic_ostream<wchar_t, std::char_traits<wchar_t>> {
public:
	basic_dostream() = delete;

	basic_dostream(void (*outputFunc)(const wchar_t *)) : std::basic_ostream<wchar_t, std::char_traits<wchar_t>>(new debugstreambuf(outputFunc)) {
	}

	virtual ~basic_dostream() {
		delete rdbuf();
	}
};

basic_dostream debug(OutputDebugStringW);
basic_dostream error(OutputDebugStringW);

struct shared_memory* shm;

static std::wstring translateError(DWORD error) {
	std::wstring buf;
	buf.resize(512);
	DWORD n = FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM, NULL, error, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), buf.data(), DWORD(buf.size()), NULL);
	if (n == 0) {
		std::wstringstream s;
		s << "Failed to translate error " << error;
		return s.str();
	}
	buf.resize(n);
	return buf;
}

void setServers(std::list<server_entry> servers) {
	memset(shm, 0, sizeof(shared_memory));
	server_entry* p = shm->servers;
	for (auto i = servers.begin(); i != servers.end(); i++) {
		*p = *i;
		p++;
	}
}

std::string getDialogItemText(HWND hwnd, int item) {
	std::string ret;
	ret.resize(512);
	UINT n = GetDlgItemTextA(hwnd, item, ret.data(), ret.size());
	ret.resize(n);
	return ret;
}

int getDialogItemInt(HWND hwnd, int item) {
	return GetDlgItemInt(hwnd, item, NULL, TRUE);
}

UINT32 parseIP(const std::string& s) {
	in_addr ip;
	if (inet_pton(AF_INET, s.c_str(), &ip) != 1) {
		DWORD err = WSAGetLastError();
		error << "inet_pton failed: " << translateError(err) << std::endl;
		return 0;
	}
	return ip.S_un.S_addr;
}

void setServerFromDialog(HWND hwnd) {
	std::string hostname = getDialogItemText(hwnd, IDC_SERVERADDR);
	int port = getDialogItemInt(hwnd, IDC_SERVERPORT);

	if (port < 1 || port > UINT16_MAX) {
		MessageBoxW(hwnd, L"Invalid port", L"Error", MB_ICONERROR);
		return;
	}

	UINT32 ip = parseIP(hostname);
	if (ip == 0) {
		MessageBoxW(hwnd, L"Invalid address", L"Error", MB_ICONERROR);
		return;
	}

	server_entry s = {
		.ip = parseIP(hostname),
		.port = htons(port)
	};

	std::stringstream ssname;
	ssname << "Direct connect (" << hostname << ":" << port << ")";
	std::string sn(ssname.str());
	sn.resize(sizeof(s.name) - 1);
	strcpy_s(s.name, sn.c_str());

	setServers({ s });
}

INT_PTR CALLBACK mainDialogProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
	switch (uMsg) {
	case WM_CLOSE:
		PostQuitMessage(0);
		return TRUE;

	case WM_COMMAND:
		switch (LOWORD(wParam)) {
		case IDC_BUTTON1:
			setServerFromDialog(hwnd);
		}
	}
	return FALSE;
}

static bool isACCWindow(HWND hwnd) {
	wchar_t windowNameBuffer[128];
	if (GetWindowTextW(hwnd, windowNameBuffer, 128) == 0) {
		return false;
	}

	if (wcscmp(windowNameBuffer, L"AC2  ") != 0) {
		return false;
	}

	wchar_t classNameBuffer[128];
	if (GetClassNameW(hwnd, classNameBuffer, 128) == 0) {
		return false;
	}

	if (wcscmp(classNameBuffer, L"UnrealWindow") != 0) {
		return false;
	}

	return true;
}

static void injectHookDLL(DWORD pid) {
	debug << "Attempting to inject hook DLL to ACC process " << pid << std::endl;

	HANDLE processHandle = OpenProcess(
		PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE,
		FALSE,
		pid);
	if (processHandle == NULL) {
		DWORD err = GetLastError();
		error << "OpenProcess failed: " << translateError(err) << std::endl;
		return;
	}

	debug << "Opened process" << std::endl;

	const wchar_t* dllPath = hookDLLPath.c_str();
	size_t dllPathLenBytes = (wcslen(dllPath) + 1) * sizeof(wchar_t);

	LPVOID dllAddr = VirtualAllocEx(
		processHandle,
		NULL,
		dllPathLenBytes,
		MEM_RESERVE | MEM_COMMIT,
		PAGE_EXECUTE_READWRITE);
	if (dllAddr == NULL) {
		DWORD err = GetLastError();
		error << "VirtualAllocEx failed: " << translateError(err) << std::endl;
		return;
	}

	debug << "Allocated " << dllPathLenBytes << " bytes for DLL path in target process at address " << dllAddr << std::endl;

	if (WriteProcessMemory(processHandle, dllAddr, dllPath, dllPathLenBytes, NULL) == FALSE) {
		DWORD err = GetLastError();
		error << "WriteProcessMemory failed: " << translateError(err) << std::endl;
		return;
	}

	debug << "Wrote DLL path to target process" << std::endl;

	LPVOID loadLibraryWAddr = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryW");

	HANDLE remoteThreadHandle = CreateRemoteThread(
		processHandle,
		NULL,
		0,
		(LPTHREAD_START_ROUTINE)loadLibraryWAddr,
		dllAddr,
		0,
		NULL);
	if (remoteThreadHandle == NULL) {
		DWORD err = GetLastError();
		error << "CreateRemoteThread failed: " << err << std::endl;
		return;
	}

	debug << "Attempted to load DLL into remote process" << std::endl;

	DWORD waitResult = WaitForSingleObject(remoteThreadHandle, 500);
	if (waitResult == WAIT_FAILED) {
		DWORD err = GetLastError();
		error << "WaitForSingleObject failed: " << translateError(err) << std::endl;
		return;
	}
	else if (waitResult == WAIT_TIMEOUT) {
		error << "WaitForSingleObject timed out" << std::endl;
		return;
	}

	DWORD exitCode;
	GetExitCodeThread(remoteThreadHandle, &exitCode);
	debug << "Remote thread exited with code " << exitCode << std::endl;

	if (exitCode != 0) {
		debug << "Successfully injected hook" << std::endl;
	}
}

BOOL CALLBACK enumProc(HWND hwnd, LPARAM lParam) {
	if (isACCWindow(hwnd)) {
		DWORD pid;
		if (GetWindowThreadProcessId(hwnd, &pid) != 0) {
			injectHookDLL(pid);
		}
	}
	return TRUE;
}

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, PWSTR cmdLine, int cmdShow) {

	HWND hwnd = CreateDialogW(hInstance, MAKEINTRESOURCE(IDD_MAIN), NULL, mainDialogProc);

	ShowWindow(hwnd, cmdShow);

	if (CreateMutexW(NULL, TRUE, L"acc-connector-event") == INVALID_HANDLE_VALUE) {
		DWORD err = GetLastError();
		error << "CreateMutexW failed: " << translateError(err) << std::endl;
		return 1;
	}

	HANDLE shmHandle = CreateFileMappingW(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(shared_memory), SHARED_MEMORY_NAME);
	if (shmHandle == NULL) {
		DWORD err = GetLastError();
		error << "CreateFileMappingW failed: " << translateError(err) << std::endl;
		return 1;
	}

	shm = (shared_memory*)MapViewOfFile(shmHandle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(shared_memory));
	memset(shm, 0, sizeof(shm));

	EnumWindows(enumProc, 0);

	MSG msg;
	while (GetMessage(&msg, NULL, 0, 0) > 0) {
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	DestroyWindow(hwnd);

	return 0;
}
