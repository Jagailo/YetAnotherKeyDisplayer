// This is the main DLL file.

#include "stdafx.h"

#include "OSD.h"

#define TICKS_PER_MICROSECOND 10
#define RTSS_VERSION(x, y) ((x << 16) + y)

namespace RTSS {

    ///<param name="entryName">
    ///The name of the OSD entry. Should be unique and not more than 255 chars once converted to ANSI.
    ///</param>
    OSD::OSD(String^ entryName)
    {
        if( String::IsNullOrWhiteSpace(entryName) )
            throw gcnew ArgumentException("Entry name cannot be null, empty, or whitespace", "entryName");

        m_entryName = (LPCSTR)Marshal::StringToHGlobalAnsi(entryName).ToPointer();
        if( strlen(m_entryName) > 255 )
            throw gcnew ArgumentException("Entry name exceeds max length of 255 when converted to ANSI", "entryName");

        //just open/close to make sure RTSS is working
        HANDLE hMapFile = NULL;
        LPRTSS_SHARED_MEMORY pMem = NULL;
        openSharedMemory(&hMapFile, &pMem);
        closeSharedMemory(hMapFile, pMem);

        m_osdSlot = 0;
        m_disposed = false;
    }

    OSD::~OSD()
    {
        if( m_disposed )
            return;

        

        //delete managed, if any

        this->!OSD();
        m_disposed = true;
    }

    OSD::!OSD()
    {
        HANDLE hMapFile = NULL;
        LPRTSS_SHARED_MEMORY pMem = NULL;
        openSharedMemory(&hMapFile, &pMem);

        //find entries and zero them out
        for(DWORD i=1; i < pMem->dwOSDArrSize; i++)
        {
            //calc offset of entry
            auto pEntry = (RTSS_SHARED_MEMORY::LPRTSS_SHARED_MEMORY_OSD_ENTRY)( (LPBYTE)pMem + pMem->dwOSDArrOffset + (i * pMem->dwOSDEntrySize) );

            if( STRMATCHES(strcmp(pEntry->szOSDOwner, m_entryName)) )
            {
                SecureZeroMemory(pEntry, pMem->dwOSDEntrySize); //won't get optimized away
                pMem->dwOSDFrame++; //forces OSD update
            }
        }

        closeSharedMemory(hMapFile, pMem);
        Marshal::FreeHGlobal(IntPtr((LPVOID)m_entryName));
    }

    System::Version^ OSD::Version::get()
    {
        HANDLE hMapFile = NULL;
        LPRTSS_SHARED_MEMORY pMem = NULL;
        openSharedMemory(&hMapFile, &pMem);

        auto ver = gcnew System::Version(pMem->dwVersion >> 16, pMem->dwVersion & 0xFFFF);

        closeSharedMemory(hMapFile, pMem);
        return ver;
    }

	DWORD OSD::EmbedGraphArray(DWORD dwOffset, FLOAT lpBuffer, DWORD dwBufferPos, DWORD dwBufferSize, LONG dwWidth, LONG dwHeight, LONG dwMargin, FLOAT fltMin, FLOAT fltMax, DWORD dwFlags)
	{
		return EmbedGraph(dwOffset, &lpBuffer, dwBufferPos, dwBufferSize, dwWidth, dwHeight, dwMargin, fltMin, fltMax, dwFlags);
	}

	/////////////////////////////////////////////////////////////////////////////
	DWORD OSD::EmbedGraph(DWORD dwOffset, FLOAT* lpBuffer, DWORD dwBufferPos, DWORD dwBufferSize, LONG dwWidth, LONG dwHeight, LONG dwMargin, FLOAT fltMin, FLOAT fltMax, DWORD dwFlags)
	{
		DWORD dwResult = 0;

		HANDLE hMapFile = NULL;
		LPRTSS_SHARED_MEMORY pMem = NULL;
		openSharedMemory(&hMapFile, &pMem);

		if (hMapFile)
		{
			LPVOID pMapAddr = MapViewOfFile(hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, 0);
			LPRTSS_SHARED_MEMORY pMem = (LPRTSS_SHARED_MEMORY)pMapAddr;

			if (pMem)
			{
					for (DWORD dwPass = 0; dwPass<2; dwPass++)
						//1st pass : find previously captured OSD slot
						//2nd pass : otherwise find the first unused OSD slot and capture it
					{
						for (DWORD dwEntry = 1; dwEntry<pMem->dwOSDArrSize; dwEntry++)
							//allow primary OSD clients (i.e. EVGA Precision / MSI Afterburner) to use the first slot exclusively, so third party
							//applications start scanning the slots from the second one
						{
							auto pEntry = (RTSS_SHARED_MEMORY::LPRTSS_SHARED_MEMORY_OSD_ENTRY)((LPBYTE)pMem + pMem->dwOSDArrOffset + (dwEntry * pMem->dwOSDEntrySize));

							if (dwPass)
							{
								if (!strlen(pEntry->szOSDOwner))
									strcpy_s(pEntry->szOSDOwner, sizeof(pEntry->szOSDOwner), m_entryName);
							}

							if (!strcmp(pEntry->szOSDOwner, m_entryName))
							{
								if (pMem->dwVersion >= 0x0002000c)
									//embedded graphs are supported for v2.12 and higher shared memory
								{
									if (dwOffset + sizeof(RTSS_EMBEDDED_OBJECT_GRAPH) + dwBufferSize * sizeof(FLOAT) > sizeof(pEntry->buffer))
										//validate embedded object offset and size and ensure that we don't overrun the buffer
									{
										UnmapViewOfFile(pMapAddr);

										CloseHandle(hMapFile);

										return 0;
									}

									LPRTSS_EMBEDDED_OBJECT_GRAPH lpGraph = (LPRTSS_EMBEDDED_OBJECT_GRAPH)(pEntry->buffer + dwOffset);
									//get pointer to object in buffer

									lpGraph->header.dwSignature = RTSS_EMBEDDED_OBJECT_GRAPH_SIGNATURE;
									lpGraph->header.dwSize = sizeof(RTSS_EMBEDDED_OBJECT_GRAPH) + dwBufferSize * sizeof(FLOAT);
									lpGraph->header.dwWidth = dwWidth;
									lpGraph->header.dwHeight = dwHeight;
									lpGraph->header.dwMargin = dwMargin;
									lpGraph->dwFlags = dwFlags;
									lpGraph->fltMin = fltMin;
									lpGraph->fltMax = fltMax;
									lpGraph->dwDataCount = dwBufferSize;

									if (lpBuffer && dwBufferSize)
									{
										for (DWORD dwPos = 0; dwPos<dwBufferSize; dwPos++)
										{
											FLOAT fltData = lpBuffer[dwBufferPos];

											lpGraph->fltData[dwPos] = (fltData == 120) ? 0 : fltData;

											dwBufferPos = (dwBufferPos + 1) & (dwBufferSize - 1);
										}
									}

									dwResult = lpGraph->header.dwSize;
								}

								break;
							}
						}

						if (dwResult)
							break;
					}
				

				UnmapViewOfFile(pMapAddr);
			}
			
			closeSharedMemory(hMapFile, pMem);
			
		}

		return dwResult;
	}
    ///<summary>
    ///Text should be no longer than 4095 chars once converted to ANSI. Lower case looks awful.
    ///</summary>
    void OSD::Update(String^ text)
    {
        if( text == nullptr )
            throw gcnew ArgumentNullException("text");

        LPCSTR lpText = (LPCSTR)Marshal::StringToHGlobalAnsi(text).ToPointer();
        if( strlen(lpText) > 4095 )
            throw gcnew ArgumentException("Text exceeds max length of 4095 when converted to ANSI", "text");

        HANDLE hMapFile = NULL;
        LPRTSS_SHARED_MEMORY pMem = NULL;
        openSharedMemory(&hMapFile, &pMem);

        //start at either our previously used slot, or the top
        for(DWORD i=(m_osdSlot == 0 ? 1 : m_osdSlot); i < pMem->dwOSDArrSize; i++)
        {
            auto pEntry = (RTSS_SHARED_MEMORY::LPRTSS_SHARED_MEMORY_OSD_ENTRY)( (LPBYTE)pMem + pMem->dwOSDArrOffset + (i * pMem->dwOSDEntrySize) );

            //if we need a new slot and this one is unused, claim it
            if( m_osdSlot == 0 && !strlen(pEntry->szOSDOwner) )
            {
                m_osdSlot = i;
                strcpy_s(pEntry->szOSDOwner, m_entryName);
            }

            //if this is our slot
            if( STRMATCHES(strcmp(pEntry->szOSDOwner, m_entryName)) )
            {
                //use extended text slot for v2.7 and higher shared memory, it allows displaying 4096 symbols instead of 256 for regular text slot
                if( pMem->dwVersion >= RTSS_VERSION(2,7) )
                    strncpy_s(pEntry->szOSDEx, lpText, sizeof(pEntry->szOSDEx)-1);
                else
                    strncpy_s(pEntry->szOSD, lpText, sizeof(pEntry->szOSD)-1);

                pMem->dwOSDFrame++; //forces OSD update
                break;
            }

            //in case we lost our previously used slot or something, let's start over
            if( m_osdSlot != 0 )
            {
                m_osdSlot = 0;
                i = 1;
            }
        }

        closeSharedMemory(hMapFile, pMem);
        Marshal::FreeHGlobal(IntPtr((LPVOID)lpText));
    }

    array<OSDEntry^>^ OSD::GetOSDEntries()
    {
        HANDLE hMapFile = NULL;
        LPRTSS_SHARED_MEMORY pMem = NULL;
        openSharedMemory(&hMapFile, &pMem);

        auto list = gcnew List<OSDEntry^>;

        //include all slots
        for(DWORD i=0; i < pMem->dwOSDArrSize; i++)
        {
            auto pEntry = (RTSS_SHARED_MEMORY::LPRTSS_SHARED_MEMORY_OSD_ENTRY)( (LPBYTE)pMem + pMem->dwOSDArrOffset + (i * pMem->dwOSDEntrySize) );
            if( strlen(pEntry->szOSDOwner) )
            {
                auto entry = gcnew OSDEntry;
                entry->Owner = Marshal::PtrToStringAnsi(IntPtr(pEntry->szOSDOwner));

                if( pMem->dwVersion >= RTSS_VERSION(2,7) )
                    entry->Text = Marshal::PtrToStringAnsi(IntPtr(pEntry->szOSDEx));
                else
                    entry->Text = Marshal::PtrToStringAnsi(IntPtr(pEntry->szOSD));

                list->Add(entry);
            }
        }

        closeSharedMemory(hMapFile, pMem);
        return list->ToArray();
    }

    array<AppEntry^>^ OSD::GetAppEntries()
    {
        HANDLE hMapFile = NULL;
        LPRTSS_SHARED_MEMORY pMem = NULL;
        openSharedMemory(&hMapFile, &pMem);

        auto list = gcnew List<AppEntry^>;

        //include all slots
        for(DWORD i=0; i < pMem->dwAppArrSize; i++)
        {
            auto pEntry = (RTSS_SHARED_MEMORY::LPRTSS_SHARED_MEMORY_APP_ENTRY)( (LPBYTE)pMem + pMem->dwAppArrOffset + (i * pMem->dwAppEntrySize) );
            if( pEntry->dwProcessID )
            {
                auto entry = gcnew AppEntry;
                
                //basic fields
                entry->ProcessId = pEntry->dwProcessID;
                entry->Name = Marshal::PtrToStringAnsi(IntPtr(pEntry->szName));
                entry->Flags = (AppFlags)pEntry->dwFlags;

                //instantaneous framerate fields
                entry->InstantaneousTimeStart = timeFromTickcount(pEntry->dwTime0);
                entry->InstantaneousTimeEnd = timeFromTickcount(pEntry->dwTime1);
                entry->InstantaneousFrames = pEntry->dwFrames;
                entry->InstantaneousFrameTime = TimeSpan::FromTicks(pEntry->dwFrameTime * TICKS_PER_MICROSECOND);

                //framerate stats fields
                entry->StatFlags = (StatFlags)pEntry->dwStatFlags;
                entry->StatTimeStart = timeFromTickcount(pEntry->dwStatTime0);
                entry->StatTimeEnd = timeFromTickcount(pEntry->dwStatTime1);
                entry->StatFrames = pEntry->dwStatFrames;
                entry->StatCount = pEntry->dwStatCount;
                entry->StatFramerateMin = pEntry->dwStatFramerateMin;
                entry->StatFramerateAvg = pEntry->dwStatFramerateAvg;
                entry->StatFramerateMax = pEntry->dwStatFramerateMax;
                if( pMem->dwVersion >= RTSS_VERSION(2,5) )
                {
                    entry->StatFrameTimeMin = pEntry->dwStatFrameTimeMin;
                    entry->StatFrameTimeAvg = pEntry->dwStatFrameTimeAvg;
                    entry->StatFrameTimeMax = pEntry->dwStatFrameTimeMax;
                    entry->StatFrameTimeCount = pEntry->dwStatFrameTimeCount;
                    //TODO - frametime buffer?
                }

                //OSD fields
                entry->OSDCoordinateX = pEntry->dwOSDX;
                entry->OSDCoordinateY = pEntry->dwOSDY;
                entry->OSDZoom = pEntry->dwOSDPixel;
                entry->OSDFrameId = pEntry->dwOSDFrame;
                entry->OSDColor = Color::FromArgb(pEntry->dwOSDColor);
                if( pMem->dwVersion >= RTSS_VERSION(2,1) )
                    entry->OSDBackgroundColor = Color::FromArgb(pEntry->dwOSDBgndColor);

                //screenshot fields
                entry->ScreenshotFlags = (ScreenshotFlags)pEntry->dwScreenCaptureFlags;
                entry->ScreenshotPath = Marshal::PtrToStringAnsi(IntPtr(pEntry->szScreenCapturePath));
                if( pMem->dwVersion >= RTSS_VERSION(2,2) )
                {
                    entry->ScreenshotQuality = pEntry->dwScreenCaptureQuality;
                    entry->ScreenshotThreads = pEntry->dwScreenCaptureThreads;
                }

                //video capture fields
                if( pMem->dwVersion >= RTSS_VERSION(2,2) )
                {
                    entry->VideoCaptureFlags = (VideoCaptureFlags)pEntry->dwVideoCaptureFlags;
                    entry->VideoCapturePath = Marshal::PtrToStringAnsi(IntPtr(pEntry->szVideoCapturePath));
                    entry->VideoFramerate = pEntry->dwVideoFramerate;
                    entry->VideoFramesize = pEntry->dwVideoFramesize;
                    entry->VideoFormat = pEntry->dwVideoFormat;
                    entry->VideoQuality = pEntry->dwVideoQuality;
                    entry->VideoCaptureThreads = pEntry->dwVideoCaptureThreads;
                }
                if( pMem->dwVersion >= RTSS_VERSION(2,4) )
                    entry->VideoCaptureFlagsEx = pEntry->dwVideoCaptureFlagsEx;

                //audio capture fields
                if( pMem->dwVersion >= RTSS_VERSION(2,3) )
                    entry->AudioCaptureFlags = pEntry->dwAudioCaptureFlags;
                if( pMem->dwVersion >= RTSS_VERSION(2,5) )
                    entry->AudioCaptureFlags2 = pEntry->dwAudioCaptureFlags2;
                if( pMem->dwVersion >= RTSS_VERSION(2,6) )
                {
                    entry->AudioCapturePTTEventPush = pEntry->qwAudioCapturePTTEventPush.QuadPart;
                    entry->AudioCapturePTTEventRelease = pEntry->qwAudioCapturePTTEventRelease.QuadPart;
                    entry->AudioCapturePTTEventPush2 = pEntry->qwAudioCapturePTTEventPush2.QuadPart;
                    entry->AudioCapturePTTEventRelease2 = pEntry->qwAudioCapturePTTEventRelease2.QuadPart;
                }

                list->Add(entry);
            }
        }

        closeSharedMemory(hMapFile, pMem);
        return list->ToArray();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void OSD::openSharedMemory(HANDLE* phMapFile, LPRTSS_SHARED_MEMORY* ppMem)
    {
        HANDLE hMapFile = NULL;
        LPRTSS_SHARED_MEMORY pMem = NULL;
        try
        {
            hMapFile = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, L"RTSSSharedMemoryV2");
            if( !hMapFile )
                THROW_LAST_ERROR();

            pMem = (LPRTSS_SHARED_MEMORY)MapViewOfFile(hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, 0);
            if( !pMem )
                THROW_LAST_ERROR();

            if( !(pMem->dwSignature == 'RTSS' && pMem->dwVersion >= RTSS_VERSION(2,0)) )
                throw gcnew System::IO::InvalidDataException("Failed to validate RTSS Shared Memory structure");

            *phMapFile = hMapFile;
            *ppMem = pMem;
        }
        catch(...)
        {
            closeSharedMemory(hMapFile, pMem);
            throw;
        }
    }

    void OSD::closeSharedMemory(HANDLE hMapFile, LPRTSS_SHARED_MEMORY pMem)
    {
        if( pMem )
            UnmapViewOfFile(pMem);

        if( hMapFile )
            CloseHandle(hMapFile);

    }

    DateTime OSD::timeFromTickcount(DWORD ticks)
    {
        return DateTime::Now - TimeSpan::FromMilliseconds(ticks);
    }
}