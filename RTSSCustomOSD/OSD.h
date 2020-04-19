#pragma once

#include "Structs.h"

using namespace System;
using namespace Collections::Generic;
using namespace Runtime::InteropServices;

namespace RTSSSharedMemoryNET
{
    public ref class OSD
    {
        LPCSTR m_entryName;
        DWORD m_osdSlot;
        bool m_disposed;

    public:
        OSD(String^ entryName);
        ~OSD();
        !OSD();

        static property Version^ Version
        {
            System::Version^ get();
        }

        void Update(String^ text);
        static array<OSDEntry^>^ GetOSDEntries();
        static array<AppEntry^>^ GetAppEntries();

    private:
        static void openSharedMemory(HANDLE* phMapFile, LPRTSS_SHARED_MEMORY* ppMem);
        static void closeSharedMemory(HANDLE hMapFile, LPRTSS_SHARED_MEMORY pMem);
        static DateTime timeFromTickCount(DWORD ticks);
    };

    LPCWSTR MBtoWC(const char* str);
}
