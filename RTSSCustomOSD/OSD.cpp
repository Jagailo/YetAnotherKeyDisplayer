#include "stdafx.h"
#include "OSD.h"

#define TICKS_PER_MICROSECOND 10
#define RTSS_VERSION(x, y) ((x << 16) + y)

namespace RTSSSharedMemoryNET {

    /// <param name="entryName">The name of the OSD entry. Should be unique and not more than 255 chars once converted to ANSI</param>
    OSD::OSD(String^ entryName)
    {
        if (String::IsNullOrWhiteSpace(entryName))
        {
            throw gcnew ArgumentException("Entry name cannot be null, empty, or whitespace", "entryName");
        }
        
        m_entryName = static_cast<LPCSTR>(Marshal::StringToHGlobalAnsi(entryName).ToPointer());
        if (strlen(m_entryName) > 255)
        {
            throw gcnew ArgumentException("Entry name exceeds max length of 255 when converted to ANSI", "entryName");
        }
        
        // Just open / close to make sure RTSS is working
        HANDLE hMapFile = nullptr;
        LPRTSS_SHARED_MEMORY pMem = nullptr;
        openSharedMemory(&hMapFile, &pMem);
        closeSharedMemory(hMapFile, pMem);

        m_osdSlot = 0;
        m_disposed = false;
    }

    OSD::~OSD()
    {
        if (m_disposed)
        {
            return;
        }

        this -> !OSD();
        m_disposed = true;
    }

    OSD::!OSD()
    {
        HANDLE hMapFile = nullptr;
        LPRTSS_SHARED_MEMORY pMem = nullptr;
        openSharedMemory(&hMapFile, &pMem);

        // Find entries and zero them out
        for (DWORD i = 1; i < pMem -> dwOSDArrSize; i++)
        {
            // Calc offset of entry
            auto pEntry = reinterpret_cast<RTSS_SHARED_MEMORY::LPRTSS_SHARED_MEMORY_OSD_ENTRY>(reinterpret_cast<LPBYTE>(pMem) + pMem -> dwOSDArrOffset + i * pMem -> dwOSDEntrySize);

            if (STRMATCHES(strcmp(pEntry -> szOSDOwner, m_entryName)))
            {
                SecureZeroMemory(pEntry, pMem -> dwOSDEntrySize); // Won't get optimized away
                pMem -> dwOSDFrame++; // Forces OSD update
            }
        }

        closeSharedMemory(hMapFile, pMem);
        Marshal::FreeHGlobal(IntPtr(LPVOID(m_entryName)));
    }

    Version^ OSD::Version::get()
    {
        HANDLE hMapFile = nullptr;
        LPRTSS_SHARED_MEMORY pMem = nullptr;
        openSharedMemory(&hMapFile, &pMem);

        const auto ver = gcnew System::Version(pMem -> dwVersion >> 16, pMem -> dwVersion & 0xFFFF);

        closeSharedMemory(hMapFile, pMem);
        return ver;
    }

    /// <summary>
    /// Text should be no longer than 4095 chars once converted to ANSI. Lower case looks awful.
    /// </summary>
    void OSD::Update(String^ text)
    {
        if (text == nullptr)
        {
            throw gcnew ArgumentNullException("text");
        }

        const LPCSTR lpText = static_cast<LPCSTR>(Marshal::StringToHGlobalAnsi(text).ToPointer());
        if (strlen(lpText) > 4095)
        {
            throw gcnew ArgumentException("Text exceeds max length of 4095 when converted to ANSI", "text");
        }
        
        HANDLE hMapFile = nullptr;
        LPRTSS_SHARED_MEMORY pMem = nullptr;
        openSharedMemory(&hMapFile, &pMem);

        // Start at either our previously used slot, or the top
        for (DWORD i = m_osdSlot == 0 ? 1 : m_osdSlot; i < pMem -> dwOSDArrSize; i++)
        {
            auto pEntry = reinterpret_cast<RTSS_SHARED_MEMORY::LPRTSS_SHARED_MEMORY_OSD_ENTRY>(reinterpret_cast<LPBYTE>(pMem) + pMem -> dwOSDArrOffset + i * pMem -> dwOSDEntrySize);

            // If we need a new slot and this one is unused, claim it
            if (m_osdSlot == 0 && !strlen(pEntry -> szOSDOwner))
            {
                m_osdSlot = i;
                strcpy_s(pEntry -> szOSDOwner, m_entryName);
            }

            // If this is our slot
            if (STRMATCHES(strcmp(pEntry -> szOSDOwner, m_entryName)))
            {
                // Use extended text slot for v2.7 and higher shared memory, it allows displaying 4096 symbols instead of 256 for regular text slot
                if (pMem -> dwVersion >= RTSS_VERSION(2, 7))
                {
                    strncpy_s(pEntry -> szOSDEx, lpText, sizeof pEntry -> szOSDEx - 1);
                }                    
                else
                {
                    strncpy_s(pEntry -> szOSD, lpText, sizeof pEntry -> szOSD - 1);
                }
                
                pMem -> dwOSDFrame++; // Forces OSD update
                break;
            }

            // In case we lost our previously used slot or something, let's start over
            if (m_osdSlot != 0)
            {
                m_osdSlot = 0;
                i = 1;
            }
        }

        closeSharedMemory(hMapFile, pMem);
        Marshal::FreeHGlobal(IntPtr(LPVOID(lpText)));
    }

    array<OSDEntry^>^ OSD::GetOSDEntries()
    {
        HANDLE hMapFile = nullptr;
        LPRTSS_SHARED_MEMORY pMem = nullptr;
        openSharedMemory(&hMapFile, &pMem);

        auto list = gcnew List<OSDEntry^>;

        // Include all slots
        for (DWORD i = 0; i < pMem -> dwOSDArrSize; i++)
        {
            auto pEntry = reinterpret_cast<RTSS_SHARED_MEMORY::LPRTSS_SHARED_MEMORY_OSD_ENTRY>(reinterpret_cast<LPBYTE>(pMem) + pMem -> dwOSDArrOffset + i * pMem -> dwOSDEntrySize);
            if (strlen(pEntry -> szOSDOwner))
            {
                const auto entry = gcnew OSDEntry;
                entry -> Owner = Marshal::PtrToStringAnsi(IntPtr(pEntry -> szOSDOwner));

                if (pMem -> dwVersion >= RTSS_VERSION(2, 7))
                {
                    entry -> Text = Marshal::PtrToStringAnsi(IntPtr(pEntry -> szOSDEx));
                }
                else
                {
                    entry -> Text = Marshal::PtrToStringAnsi(IntPtr(pEntry -> szOSD));
                }
                
                list -> Add(entry);
            }
        }

        closeSharedMemory(hMapFile, pMem);
        return list -> ToArray();
    }

    array<AppEntry^>^ OSD::GetAppEntries()
    {
        HANDLE hMapFile = nullptr;
        LPRTSS_SHARED_MEMORY pMem = nullptr;
        openSharedMemory(&hMapFile, &pMem);

        auto list = gcnew List<AppEntry^>;

        // Include all slots
        for (DWORD i = 0; i < pMem -> dwAppArrSize; i++)
        {
            auto pEntry = reinterpret_cast<RTSS_SHARED_MEMORY::LPRTSS_SHARED_MEMORY_APP_ENTRY>(reinterpret_cast<LPBYTE>(pMem) + pMem -> dwAppArrOffset + i * pMem -> dwAppEntrySize);
            if (pEntry -> dwProcessID)
            {
                const auto entry = gcnew AppEntry;

                // Basic fields
                entry -> ProcessId = pEntry -> dwProcessID;
                entry -> Name = Marshal::PtrToStringAnsi(IntPtr(pEntry -> szName));
                entry -> Flags = static_cast<AppFlags>(pEntry -> dwFlags);

                // Instantaneous framerate fields
                entry -> InstantaneousTimeStart = timeFromTickCount(pEntry -> dwTime0);
                entry -> InstantaneousTimeEnd = timeFromTickCount(pEntry -> dwTime1);
                entry -> InstantaneousFrames = pEntry -> dwFrames;
                entry -> InstantaneousFrameTime = TimeSpan::FromTicks(pEntry -> dwFrameTime * TICKS_PER_MICROSECOND);

                // Framerate stats fields
                entry -> StatFlags = static_cast<StatFlags>(pEntry -> dwStatFlags);
                entry -> StatTimeStart = timeFromTickCount(pEntry -> dwStatTime0);
                entry -> StatTimeEnd = timeFromTickCount(pEntry -> dwStatTime1);
                entry -> StatFrames = pEntry -> dwStatFrames;
                entry -> StatCount = pEntry -> dwStatCount;
                entry -> StatFramerateMin = pEntry -> dwStatFramerateMin;
                entry -> StatFramerateAvg = pEntry -> dwStatFramerateAvg;
                entry -> StatFramerateMax = pEntry -> dwStatFramerateMax;

                if (pMem -> dwVersion >= RTSS_VERSION(2, 5))
                {
                    entry -> StatFrameTimeMin = pEntry -> dwStatFrameTimeMin;
                    entry -> StatFrameTimeAvg = pEntry -> dwStatFrameTimeAvg;
                    entry -> StatFrameTimeMax = pEntry -> dwStatFrameTimeMax;
                    entry -> StatFrameTimeCount = pEntry -> dwStatFrameTimeCount;
                    // TODO: Frametime buffer?
                }

                // OSD fields
                entry -> OSDCoordinateX = pEntry -> dwOSDX;
                entry -> OSDCoordinateY = pEntry -> dwOSDY;
                entry -> OSDZoom = pEntry -> dwOSDPixel;
                entry -> OSDFrameId = pEntry -> dwOSDFrame;
                entry -> OSDColor = Color::FromArgb(pEntry -> dwOSDColor);

                if (pMem -> dwVersion >= RTSS_VERSION(2, 1))
                {
                    entry -> OSDBackgroundColor = Color::FromArgb(pEntry -> dwOSDBgndColor);
                }

                // Screenshot fields
                entry -> ScreenshotFlags = static_cast<ScreenshotFlags>(pEntry -> dwScreenCaptureFlags);
                entry -> ScreenshotPath = Marshal::PtrToStringAnsi(IntPtr(pEntry -> szScreenCapturePath));

                if (pMem -> dwVersion >= RTSS_VERSION(2, 2))
                {
                    entry -> ScreenshotQuality = pEntry -> dwScreenCaptureQuality;
                    entry -> ScreenshotThreads = pEntry -> dwScreenCaptureThreads;
                }

                // Video capture fields
                if (pMem -> dwVersion >= RTSS_VERSION(2, 2))
                {
                    entry -> VideoCaptureFlags = static_cast<VideoCaptureFlags>(pEntry -> dwVideoCaptureFlags);
                    entry -> VideoCapturePath = Marshal::PtrToStringAnsi(IntPtr(pEntry -> szVideoCapturePath));
                    entry -> VideoFramerate = pEntry -> dwVideoFramerate;
                    entry -> VideoFramesize = pEntry -> dwVideoFramesize;
                    entry -> VideoFormat = pEntry -> dwVideoFormat;
                    entry -> VideoQuality = pEntry -> dwVideoQuality;
                    entry -> VideoCaptureThreads = pEntry -> dwVideoCaptureThreads;
                }

                if (pMem -> dwVersion >= RTSS_VERSION(2, 4))
                {
                    entry -> VideoCaptureFlagsEx = pEntry -> dwVideoCaptureFlagsEx;
                }

                // Audio capture fields
                if (pMem -> dwVersion >= RTSS_VERSION(2, 3))
                {
                    entry -> AudioCaptureFlags = pEntry -> dwAudioCaptureFlags;
                }

                if (pMem -> dwVersion >= RTSS_VERSION(2, 5))
                {
                    entry -> AudioCaptureFlags2 = pEntry -> dwAudioCaptureFlags2;
                }

                if (pMem -> dwVersion >= RTSS_VERSION(2, 6))
                {
                    entry -> AudioCapturePTTEventPush = pEntry -> qwAudioCapturePTTEventPush.QuadPart;
                    entry -> AudioCapturePTTEventRelease = pEntry -> qwAudioCapturePTTEventRelease.QuadPart;
                    entry -> AudioCapturePTTEventPush2 = pEntry -> qwAudioCapturePTTEventPush2.QuadPart;
                    entry -> AudioCapturePTTEventRelease2 = pEntry -> qwAudioCapturePTTEventRelease2.QuadPart;
                }

                list -> Add(entry);
            }
        }

        closeSharedMemory(hMapFile, pMem);
        return list -> ToArray();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void OSD::openSharedMemory(HANDLE* phMapFile, LPRTSS_SHARED_MEMORY* ppMem)
    {
        HANDLE hMapFile = nullptr;
        LPRTSS_SHARED_MEMORY pMem = nullptr;
        try
        {
            hMapFile = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, L"RTSSSharedMemoryV2");
            if (!hMapFile)
            {
                THROW_LAST_ERROR();
            }

            pMem = static_cast<LPRTSS_SHARED_MEMORY>(MapViewOfFile(hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, 0));
            if (!pMem)
            {
                THROW_LAST_ERROR();
            }

            if (!(pMem -> dwSignature == 'RTSS' && pMem -> dwVersion >= RTSS_VERSION(2, 0)))
            {
                throw gcnew IO::InvalidDataException("Failed to validate RTSS Shared Memory structure");
            }

            *phMapFile = hMapFile;
            *ppMem = pMem;
        }
        catch (...)
        {
            closeSharedMemory(hMapFile, pMem);
            throw;
        }
    }

    void OSD::closeSharedMemory(HANDLE hMapFile, LPRTSS_SHARED_MEMORY pMem)
    {
        if (pMem)
        {
            UnmapViewOfFile(pMem);
        }
    	
        if (hMapFile)
        {
            CloseHandle(hMapFile);
        }
    }

    DateTime OSD::timeFromTickCount(DWORD ticks)
    {
        return DateTime::Now - TimeSpan::FromMilliseconds(ticks);
    }
}
