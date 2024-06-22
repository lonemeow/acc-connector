#include "client-hooks.h"

#include <Shlobj.h>
#include <Share.h>
#include <stdio.h>

FILE* log_file = NULL;

void initLog() {
    wchar_t *documents_path;
    HRESULT res = SHGetKnownFolderPath(&FOLDERID_Documents, 0, NULL, &documents_path);
    if (SUCCEEDED(res)) {
        wchar_t path[512];
        swprintf_s(path, 512, L"%s\\ACC Connector", documents_path);
        CreateDirectory(path, NULL);
        swprintf_s(path, 512, L"%s\\ACC Connector\\logs", documents_path);
        CreateDirectory(path, NULL);
		swprintf_s(path, 512, L"%s\\ACC Connector\\logs\\hook.log", documents_path);
        log_file = _wfsopen(path, L"a", _SH_DENYWR);
        CoTaskMemFree(documents_path);
        log_msg(L"Log opened");
    }
    else {
        log_msg(L"Failed to get Documents folder location: %x", res);
    }
}

void closeLog() {
    log_msg(L"Closing log");
    fclose(log_file);
    log_file = NULL;
}

void log_msg(const wchar_t* fmt, ...) {
    va_list args;
    wchar_t buffer[512];
    va_start(args, fmt);
    vswprintf_s(buffer, 512, fmt, args);
    va_end(args);
    OutputDebugStringW(buffer);
    if (log_file) {
        SYSTEMTIME time;
        GetLocalTime(&time);
        fwprintf(log_file, L"%04d-%02d-%02d %02d:%02d:%02d.%03d %s\n",
            time.wYear, time.wMonth, time.wDay, time.wHour, time.wMinute, time.wSecond, time.wMilliseconds, buffer);
        fflush(log_file);
    }
}
