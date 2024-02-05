#include "client-hooks.h"

#define DLL_NAME L"hid.dll"

#include <Combaseapi.h>

void (*real_HidD_GetHidGuid)(LPGUID HidGuid) = NULL;

HMODULE hRealHidDll = NULL;

BOOL initProxy() {
	wchar_t dllPathBuffer[1024];

	if (GetSystemDirectoryW(dllPathBuffer, 1024) == 0) {
		log_msg(L"GetSystemDirectoryW failed");
		return FALSE;
	}

	wcscat_s(dllPathBuffer, 1024, L"\\");
	wcscat_s(dllPathBuffer, 1024, DLL_NAME);

	log_msg(L"Trying to load %s", dllPathBuffer);

	hRealHidDll = LoadLibraryW(dllPathBuffer);
	if (hRealHidDll == NULL) {
		log_msg(L"LoadLibraryW failed");
		return FALSE;
	}

	real_HidD_GetHidGuid = (void (*)(LPGUID))GetProcAddress(hRealHidDll, "HidD_GetHidGuid");
	if (real_HidD_GetHidGuid == NULL) {
		log_msg(L"GetProcAddress(\"HidD_GetHidGuid\") failed");
		FreeLibrary(hRealHidDll);
		return FALSE;
	}

	return TRUE;
}

void closeProxy() {
	if (hRealHidDll != NULL) {
		FreeLibrary(hRealHidDll);
		hRealHidDll = NULL;
	}
}

__declspec(dllexport) void HidD_GetHidGuid(LPGUID HidGuid) {
	log_msg(L"HidD_GetHidGuid(%p)", HidGuid);
	real_HidD_GetHidGuid(HidGuid);
	wchar_t buf[123];
	StringFromGUID2(HidGuid, buf, _countof(buf));
	log_msg(L"HID guid = %s", buf);
}
