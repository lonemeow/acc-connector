/* GENERATED FILE, DO NOT EDIT */

#include "client-hooks.h"

#define DLL_NAME L"hid.dll"

static HMODULE hRealDll = NULL;
UINT64 procAddrs[44];

BOOL initProxy() {
	wchar_t dllPathBuffer[1024];

	if (GetSystemDirectoryW(dllPathBuffer, 1024) == 0) {
		log_msg(L"GetSystemDirectoryW failed");
		return FALSE;
	}

	wcscat_s(dllPathBuffer, 1024, L"\\");
	wcscat_s(dllPathBuffer, 1024, DLL_NAME);

	log_msg(L"Trying to load %s", dllPathBuffer);

	hRealDll = LoadLibraryW(dllPathBuffer);
	if (hRealDll == NULL) {
		log_msg(L"LoadLibraryW failed: 0x%x", GetLastError());
		return FALSE;
	}

	procAddrs[0] = (UINT64)GetProcAddress(hRealDll, "HidD_FlushQueue");
	procAddrs[1] = (UINT64)GetProcAddress(hRealDll, "HidD_FreePreparsedData");
	procAddrs[2] = (UINT64)GetProcAddress(hRealDll, "HidD_GetAttributes");
	procAddrs[3] = (UINT64)GetProcAddress(hRealDll, "HidD_GetConfiguration");
	procAddrs[4] = (UINT64)GetProcAddress(hRealDll, "HidD_GetFeature");
	procAddrs[5] = (UINT64)GetProcAddress(hRealDll, "HidD_GetHidGuid");
	procAddrs[6] = (UINT64)GetProcAddress(hRealDll, "HidD_GetIndexedString");
	procAddrs[7] = (UINT64)GetProcAddress(hRealDll, "HidD_GetInputReport");
	procAddrs[8] = (UINT64)GetProcAddress(hRealDll, "HidD_GetManufacturerString");
	procAddrs[9] = (UINT64)GetProcAddress(hRealDll, "HidD_GetMsGenreDescriptor");
	procAddrs[10] = (UINT64)GetProcAddress(hRealDll, "HidD_GetNumInputBuffers");
	procAddrs[11] = (UINT64)GetProcAddress(hRealDll, "HidD_GetPhysicalDescriptor");
	procAddrs[12] = (UINT64)GetProcAddress(hRealDll, "HidD_GetPreparsedData");
	procAddrs[13] = (UINT64)GetProcAddress(hRealDll, "HidD_GetProductString");
	procAddrs[14] = (UINT64)GetProcAddress(hRealDll, "HidD_GetSerialNumberString");
	procAddrs[15] = (UINT64)GetProcAddress(hRealDll, "HidD_Hello");
	procAddrs[16] = (UINT64)GetProcAddress(hRealDll, "HidD_SetConfiguration");
	procAddrs[17] = (UINT64)GetProcAddress(hRealDll, "HidD_SetFeature");
	procAddrs[18] = (UINT64)GetProcAddress(hRealDll, "HidD_SetNumInputBuffers");
	procAddrs[19] = (UINT64)GetProcAddress(hRealDll, "HidD_SetOutputReport");
	procAddrs[20] = (UINT64)GetProcAddress(hRealDll, "HidP_GetButtonCaps");
	procAddrs[21] = (UINT64)GetProcAddress(hRealDll, "HidP_GetCaps");
	procAddrs[22] = (UINT64)GetProcAddress(hRealDll, "HidP_GetData");
	procAddrs[23] = (UINT64)GetProcAddress(hRealDll, "HidP_GetExtendedAttributes");
	procAddrs[24] = (UINT64)GetProcAddress(hRealDll, "HidP_GetLinkCollectionNodes");
	procAddrs[25] = (UINT64)GetProcAddress(hRealDll, "HidP_GetScaledUsageValue");
	procAddrs[26] = (UINT64)GetProcAddress(hRealDll, "HidP_GetSpecificButtonCaps");
	procAddrs[27] = (UINT64)GetProcAddress(hRealDll, "HidP_GetSpecificValueCaps");
	procAddrs[28] = (UINT64)GetProcAddress(hRealDll, "HidP_GetUsageValue");
	procAddrs[29] = (UINT64)GetProcAddress(hRealDll, "HidP_GetUsageValueArray");
	procAddrs[30] = (UINT64)GetProcAddress(hRealDll, "HidP_GetUsages");
	procAddrs[31] = (UINT64)GetProcAddress(hRealDll, "HidP_GetUsagesEx");
	procAddrs[32] = (UINT64)GetProcAddress(hRealDll, "HidP_GetValueCaps");
	procAddrs[33] = (UINT64)GetProcAddress(hRealDll, "HidP_InitializeReportForID");
	procAddrs[34] = (UINT64)GetProcAddress(hRealDll, "HidP_MaxDataListLength");
	procAddrs[35] = (UINT64)GetProcAddress(hRealDll, "HidP_MaxUsageListLength");
	procAddrs[36] = (UINT64)GetProcAddress(hRealDll, "HidP_SetData");
	procAddrs[37] = (UINT64)GetProcAddress(hRealDll, "HidP_SetScaledUsageValue");
	procAddrs[38] = (UINT64)GetProcAddress(hRealDll, "HidP_SetUsageValue");
	procAddrs[39] = (UINT64)GetProcAddress(hRealDll, "HidP_SetUsageValueArray");
	procAddrs[40] = (UINT64)GetProcAddress(hRealDll, "HidP_SetUsages");
	procAddrs[41] = (UINT64)GetProcAddress(hRealDll, "HidP_TranslateUsagesToI8042ScanCodes");
	procAddrs[42] = (UINT64)GetProcAddress(hRealDll, "HidP_UnsetUsages");
	procAddrs[43] = (UINT64)GetProcAddress(hRealDll, "HidP_UsageListDifference");

	return TRUE;
}

void closeProxy() {
	if (hRealDll != NULL) {
		FreeLibrary(hRealDll);
		hRealDll = NULL;
	}
}
