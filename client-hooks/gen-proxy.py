#!/usr/bin/env python3

import os
import sys
import pefile

dll = pefile.PE(sys.argv[1])
dll_name = os.path.basename(sys.argv[1])

tabsep = "\n\t"

exports = dll.DIRECTORY_ENTRY_EXPORT.symbols

with open("proxy.def", "w") as f:
    f.write('EXPORTS\n')
    for export in exports:
        f.write('\t{} @{}\n'.format(export.name.decode(), export.ordinal))

with open("proxy.c", "w") as f:
    f.write(
f'''/* GENERATED FILE, DO NOT EDIT */

#include "client-hooks.h"

#define DLL_NAME L"{dll_name}"

static HMODULE hRealDll = NULL;
UINT64 procAddrs[{len(exports)}];

BOOL initProxy() {{
	wchar_t dllPathBuffer[1024];

	if (GetSystemDirectoryW(dllPathBuffer, 1024) == 0) {{
		log_msg(L"GetSystemDirectoryW failed");
		return FALSE;
	}}

	wcscat_s(dllPathBuffer, 1024, L"\\\\");
	wcscat_s(dllPathBuffer, 1024, DLL_NAME);

	log_msg(L"Trying to load %s", dllPathBuffer);

	hRealDll = LoadLibraryW(dllPathBuffer);
	if (hRealDll == NULL) {{
		log_msg(L"LoadLibraryW failed: 0x%x", GetLastError());
		return FALSE;
	}}

	{tabsep.join((f'procAddrs[{i}] = (UINT64)GetProcAddress(hRealDll, "{export.name.decode()}");' for i, export in enumerate(exports)))}

	return TRUE;
}}

void closeProxy() {{
	if (hRealDll != NULL) {{
		FreeLibrary(hRealDll);
		hRealDll = NULL;
	}}
}}
''')

with open("proxy_asm.asm", "w") as f:
    f.write(
'''.code
extern procAddrs:QWORD
'''
)
    for i, export in enumerate(exports):
        name = export.name.decode()
        f.write(
f'''
{name} proc
	jmp procAddrs[{i}*8]
{name} endp
''')
    f.write('end\n')
