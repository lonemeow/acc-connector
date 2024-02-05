#pragma once

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

BOOL attachHooks();
void removeHooks();

void log_msg(const wchar_t* fmt, ...);

BOOL initProxy();
void closeProxy();
