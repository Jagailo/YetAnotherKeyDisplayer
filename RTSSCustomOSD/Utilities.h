#pragma once

//http://stackoverflow.com/a/673336/489071
template <typename T>
public ref class using_auto_ptr
{
public:
    using_auto_ptr(T^ p) : m_p(p), m_used(true)
    {
        //type check, since C#'s using is only for IDisposable
        static_cast<System::IDisposable^>(p);
    }

    ~using_auto_ptr() { delete m_p; }

    T^ operator->() { return m_p; }
    static T^ operator*(using_auto_ptr<T>% p) { return p->m_p; } //C4383 says I should do it this way

    bool m_used;

private:
    T^ m_p;
};

// USING( [TYPE], [VARIABLE NAME], [CONSTRUCTOR] )
#define USING(CLASS, VAR, ALLOC) \
    for( using_auto_ptr<CLASS> VAR(ALLOC); VAR.m_used; VAR.m_used=0)

//for easily handling unmanaged shit
#define SAFE_RELEASE(p) if( NULL != (p) ){ (p)->Release(); (p) = NULL; }
#define SAFE_DELETE(p) if( NULL != (p) ){ delete (p); (p) = NULL; }
#define GOTO_FAIL_IF_FAILED(hr) if( FAILED(hr) ){ goto fail; }

//for easily handling unmanaged to managed errors
#define THROW_HR(hr) System::Runtime::InteropServices::Marshal::ThrowExceptionForHR(hr)
#define THROW_ERROR(error) THROW_HR(HRESULT_FROM_WIN32(error))
#define THROW_LAST_ERROR() THROW_HR(System::Runtime::InteropServices::Marshal::GetHRForLastWin32Error())

//for strcmp and co
#define STRMATCHES(x) (x) == 0
#define STRLESSTHAN(x) (x) < 0
#define STRMORETHAN(x) (x) > 0
