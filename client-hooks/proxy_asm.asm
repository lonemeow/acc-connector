.code
extern procAddrs:QWORD

HidD_FlushQueue proc
	jmp procAddrs[0*8]
HidD_FlushQueue endp

HidD_FreePreparsedData proc
	jmp procAddrs[1*8]
HidD_FreePreparsedData endp

HidD_GetAttributes proc
	jmp procAddrs[2*8]
HidD_GetAttributes endp

HidD_GetConfiguration proc
	jmp procAddrs[3*8]
HidD_GetConfiguration endp

HidD_GetFeature proc
	jmp procAddrs[4*8]
HidD_GetFeature endp

HidD_GetHidGuid proc
	jmp procAddrs[5*8]
HidD_GetHidGuid endp

HidD_GetIndexedString proc
	jmp procAddrs[6*8]
HidD_GetIndexedString endp

HidD_GetInputReport proc
	jmp procAddrs[7*8]
HidD_GetInputReport endp

HidD_GetManufacturerString proc
	jmp procAddrs[8*8]
HidD_GetManufacturerString endp

HidD_GetMsGenreDescriptor proc
	jmp procAddrs[9*8]
HidD_GetMsGenreDescriptor endp

HidD_GetNumInputBuffers proc
	jmp procAddrs[10*8]
HidD_GetNumInputBuffers endp

HidD_GetPhysicalDescriptor proc
	jmp procAddrs[11*8]
HidD_GetPhysicalDescriptor endp

HidD_GetPreparsedData proc
	jmp procAddrs[12*8]
HidD_GetPreparsedData endp

HidD_GetProductString proc
	jmp procAddrs[13*8]
HidD_GetProductString endp

HidD_GetSerialNumberString proc
	jmp procAddrs[14*8]
HidD_GetSerialNumberString endp

HidD_Hello proc
	jmp procAddrs[15*8]
HidD_Hello endp

HidD_SetConfiguration proc
	jmp procAddrs[16*8]
HidD_SetConfiguration endp

HidD_SetFeature proc
	jmp procAddrs[17*8]
HidD_SetFeature endp

HidD_SetNumInputBuffers proc
	jmp procAddrs[18*8]
HidD_SetNumInputBuffers endp

HidD_SetOutputReport proc
	jmp procAddrs[19*8]
HidD_SetOutputReport endp

HidP_GetButtonCaps proc
	jmp procAddrs[20*8]
HidP_GetButtonCaps endp

HidP_GetCaps proc
	jmp procAddrs[21*8]
HidP_GetCaps endp

HidP_GetData proc
	jmp procAddrs[22*8]
HidP_GetData endp

HidP_GetExtendedAttributes proc
	jmp procAddrs[23*8]
HidP_GetExtendedAttributes endp

HidP_GetLinkCollectionNodes proc
	jmp procAddrs[24*8]
HidP_GetLinkCollectionNodes endp

HidP_GetScaledUsageValue proc
	jmp procAddrs[25*8]
HidP_GetScaledUsageValue endp

HidP_GetSpecificButtonCaps proc
	jmp procAddrs[26*8]
HidP_GetSpecificButtonCaps endp

HidP_GetSpecificValueCaps proc
	jmp procAddrs[27*8]
HidP_GetSpecificValueCaps endp

HidP_GetUsageValue proc
	jmp procAddrs[28*8]
HidP_GetUsageValue endp

HidP_GetUsageValueArray proc
	jmp procAddrs[29*8]
HidP_GetUsageValueArray endp

HidP_GetUsages proc
	jmp procAddrs[30*8]
HidP_GetUsages endp

HidP_GetUsagesEx proc
	jmp procAddrs[31*8]
HidP_GetUsagesEx endp

HidP_GetValueCaps proc
	jmp procAddrs[32*8]
HidP_GetValueCaps endp

HidP_InitializeReportForID proc
	jmp procAddrs[33*8]
HidP_InitializeReportForID endp

HidP_MaxDataListLength proc
	jmp procAddrs[34*8]
HidP_MaxDataListLength endp

HidP_MaxUsageListLength proc
	jmp procAddrs[35*8]
HidP_MaxUsageListLength endp

HidP_SetData proc
	jmp procAddrs[36*8]
HidP_SetData endp

HidP_SetScaledUsageValue proc
	jmp procAddrs[37*8]
HidP_SetScaledUsageValue endp

HidP_SetUsageValue proc
	jmp procAddrs[38*8]
HidP_SetUsageValue endp

HidP_SetUsageValueArray proc
	jmp procAddrs[39*8]
HidP_SetUsageValueArray endp

HidP_SetUsages proc
	jmp procAddrs[40*8]
HidP_SetUsages endp

HidP_TranslateUsagesToI8042ScanCodes proc
	jmp procAddrs[41*8]
HidP_TranslateUsagesToI8042ScanCodes endp

HidP_UnsetUsages proc
	jmp procAddrs[42*8]
HidP_UnsetUsages endp

HidP_UsageListDifference proc
	jmp procAddrs[43*8]
HidP_UsageListDifference endp
end
