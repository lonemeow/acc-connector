#include "client-hooks.h"

#include <stdio.h>

void log_msg(const wchar_t* fmt, ...) {
    va_list args;
    wchar_t buffer[512];
    va_start(args, fmt);
    vswprintf_s(buffer, 512, fmt, args);
    va_end(args);
    OutputDebugStringW(buffer);
}
