namespace YAKD.Enums
{
    /// <summary>
    /// Mouse messages
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/inputdev/about-mouse-input</remarks>
    public enum MouseMessage
    {
        /// <summary>
        /// The left mouse button was pressed
        /// </summary>
        WM_LBUTTONDOWN = 0x0201,

        /// <summary>
        /// The left mouse button was released
        /// </summary>
        WM_LBUTTONUP = 0x0202,

        /// <summary>
        /// The left mouse button was double-clicked
        /// </summary>
        WM_LBUTTONDBLCLK = 0x0203,

        /// <summary>
        /// The right mouse button was pressed
        /// </summary>
        WM_RBUTTONDOWN = 0x0204,

        /// <summary>
        /// The right mouse button was released
        /// </summary>
        WM_RBUTTONUP = 0x0205,

        /// <summary>
        /// The right mouse button was double-clicked
        /// </summary>
        WM_RBUTTONDBLCLK = 0x0206,

        /// <summary>
        /// The middle mouse button was pressed
        /// </summary>
        WM_MBUTTONDOWN = 0x0207,

        /// <summary>
        /// The middle mouse button was released
        /// </summary>
        WM_MBUTTONUP = 0x0208,

        /// <summary>
        /// The middle mouse button was double-clicked
        /// </summary>
        WM_MBUTTONDBLCLK = 0x0209,

        /// <summary>
        /// An X mouse button was pressed
        /// </summary>
        WM_XBUTTONDOWN = 0x020B,

        /// <summary>
        /// An X mouse button was released
        /// </summary>
        WM_XBUTTONUP = 0x020C,

        /// <summary>
        /// An X mouse button was double-clicked
        /// </summary>
        WM_XBUTTONDBLCLK = 0x020D
    }
}
