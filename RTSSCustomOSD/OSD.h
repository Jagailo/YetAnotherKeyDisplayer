
#pragma once

#include "Structs.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

namespace RTSS {

    public ref class OSD
    {
    private:
        LPCSTR m_entryName;
        DWORD m_osdSlot;
        bool m_disposed;

    public:
        OSD(String^ entryName);
        ~OSD();
        !OSD();

        static property System::Version^ Version
        {
            System::Version^ get();
        }

		DWORD EmbedGraphArray(DWORD dwOffset, FLOAT lpBuffer, DWORD dwBufferPos, DWORD dwBufferSize, LONG dwWidth, LONG dwHeight, LONG dwMargin, FLOAT fltMin, FLOAT fltMax, DWORD dwFlags);

		DWORD EmbedGraph(DWORD dwOffset, FLOAT * lpBuffer, DWORD dwBufferPos, DWORD dwBufferSize, LONG dwWidth, LONG dwHeight, LONG dwMargin, FLOAT fltMin, FLOAT fltMax, DWORD dwFlags);

		void Update(String^ text);
        static array<OSDEntry^>^ GetOSDEntries();
        static array<AppEntry^>^ GetAppEntries();

    private:
        static void openSharedMemory(HANDLE* phMapFile, LPRTSS_SHARED_MEMORY* ppMem);
        static void closeSharedMemory(HANDLE hMapFile, LPRTSS_SHARED_MEMORY pMem);
        static DateTime timeFromTickcount(DWORD ticks);
    };

    LPCWSTR MBtoWC(const char* str);
}
