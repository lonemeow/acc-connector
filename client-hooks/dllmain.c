#include "client-hooks.h"

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD fdwReason, LPVOID lpvReserved)
{
    switch (fdwReason)
    {
    case DLL_PROCESS_ATTACH:
        initLog();
        log_msg(L"DLL attaching...");
        if (!initProxy()) {
            log_msg(L"Failed to initialize proxy DLL");
            return FALSE;
        }
        if (!attachHooks()) {
            log_msg(L"Failed to attach hooks");
            return FALSE;
        }
        break;

    case DLL_PROCESS_DETACH:
        log_msg(L"DLL detaching...");
        removeHooks();
        closeProxy();
        closeLog();
        break;

    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;
    }

    return TRUE;
}
