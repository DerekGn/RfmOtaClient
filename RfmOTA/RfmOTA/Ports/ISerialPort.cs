using System;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace RfmOta.Ports
{
    internal interface ISerialPort : IDisposable
    {
        //
        // Summary:
        //     Gets or sets the handshaking protocol for serial port transmission of data using
        //     a value from System.IO.Ports.Handshake.
        //
        // Returns:
        //     One of the System.IO.Ports.Handshake values. The default is None.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The value passed is not a valid value in the System.IO.Ports.Handshake enumeration.
        public Handshake Handshake { get; set; }
        //
        // Summary:
        //     Gets or sets the byte encoding for pre- and post-transmission conversion of text.
        //
        // Returns:
        //     An System.Text.Encoding object. The default is System.Text.ASCIIEncoding.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The System.IO.Ports.SerialPort.Encoding property was set to null.
        //
        //   T:System.ArgumentException:
        //     The System.IO.Ports.SerialPort.Encoding property was set to an encoding that
        //     is not System.Text.ASCIIEncoding, System.Text.UTF8Encoding, System.Text.UTF32Encoding,
        //     System.Text.UnicodeEncoding, one of the Windows single byte encodings, or one
        //     of the Windows double byte encodings.
        public Encoding Encoding { get; set; }
        //
        // Summary:
        //     Gets or sets a value that enables the Data Terminal Ready (DTR) signal during
        //     serial communication.
        //
        // Returns:
        //     true to enable Data Terminal Ready (DTR); otherwise, false. The default is false.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        public bool DtrEnable { get; set; }
        //
        // Summary:
        //     Gets the state of the Clear-to-Send line.
        //
        // Returns:
        //     true if the Clear-to-Send line is detected; otherwise, false.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.InvalidOperationException:
        //     The stream is closed. This can occur because the System.IO.Ports.SerialPort.Open
        //     method has not been called or the System.IO.Ports.SerialPort.Close method has
        //     been called.
        public bool CtsHolding { get; }
        //
        // Summary:
        //     Gets or sets a value indicating whether null bytes are ignored when transmitted
        //     between the port and the receive buffer.
        //
        // Returns:
        //     true if null bytes are ignored; otherwise false. The default is false.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.InvalidOperationException:
        //     The stream is closed. This can occur because the System.IO.Ports.SerialPort.Open
        //     method has not been called or the System.IO.Ports.SerialPort.Close method has
        //     been called.
        public bool DiscardNull { get; set; }
        //
        // Summary:
        //     Gets or sets the standard length of data bits per byte.
        //
        // Returns:
        //     The data bits length.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The data bits value is less than 5 or more than 8.
        public int DataBits { get; set; }
        //
        // Summary:
        //     Gets a value indicating the open or closed status of the System.IO.Ports.SerialPort
        //     object.
        //
        // Returns:
        //     true if the serial port is open; otherwise, false. The default is false.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The System.IO.Ports.SerialPort.IsOpen value passed is null.
        //
        //   T:System.ArgumentException:
        //     The System.IO.Ports.SerialPort.IsOpen value passed is an empty string ("").
        public bool IsOpen { get; }
        //
        // Summary:
        //     Gets the state of the Data Set Ready (DSR) signal.
        //
        // Returns:
        //     true if a Data Set Ready signal has been sent to the port; otherwise, false.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.InvalidOperationException:
        //     The stream is closed. This can occur because the System.IO.Ports.SerialPort.Open
        //     method has not been called or the System.IO.Ports.SerialPort.Close method has
        //     been called.
        public bool DsrHolding { get; }
        //
        // Summary:
        //     Gets or sets the value used to interpret the end of a call to the System.IO.Ports.SerialPort.ReadLine
        //     and System.IO.Ports.SerialPort.WriteLine(System.String) methods.
        //
        // Returns:
        //     A value that represents the end of a line. The default is a line feed ("\n" in
        //     C# or Microsoft.VisualBasic.Constants.vbLf in Visual Basic).
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The property value is empty.
        //
        //   T:System.ArgumentNullException:
        //     The property value is null.
        public string NewLine { get; set; }
        //
        // Summary:
        //     Gets or sets the size of the System.IO.Ports.SerialPort input buffer.
        //
        // Returns:
        //     The buffer size, in bytes. The default value is 4096; the maximum value is that
        //     of a positive int, or 2147483647.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The System.IO.Ports.SerialPort.ReadBufferSize value set is less than or equal
        //     to zero.
        //
        //   T:System.InvalidOperationException:
        //     The System.IO.Ports.SerialPort.ReadBufferSize property was set while the stream
        //     was open.
        //
        //   T:System.IO.IOException:
        //     The System.IO.Ports.SerialPort.ReadBufferSize property was set to an odd integer
        //     value.
        public int ReadBufferSize { get; set; }
        //
        // Summary:
        //     Gets or sets the byte that replaces invalid bytes in a data stream when a parity
        //     error occurs.
        //
        // Returns:
        //     A byte that replaces invalid bytes.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        public byte ParityReplace { get; set; }
        //
        // Summary:
        //     Gets or sets the port for communications, including but not limited to all available
        //     COM ports.
        //
        // Returns:
        //     The communications port. The default is COM1.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The System.IO.Ports.SerialPort.PortName property was set to a value with a length
        //     of zero. -or- The System.IO.Ports.SerialPort.PortName property was set to a value
        //     that starts with "\\". -or- The port name was not valid.
        //
        //   T:System.ArgumentNullException:
        //     The System.IO.Ports.SerialPort.PortName property was set to null.
        //
        //   T:System.InvalidOperationException:
        //     The specified port is open.
        public string PortName { get; set; }
        //
        // Summary:
        //     Gets the state of the Carrier Detect line for the port.
        //
        // Returns:
        //     true if the carrier is detected; otherwise, false.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.InvalidOperationException:
        //     The stream is closed. This can occur because the System.IO.Ports.SerialPort.Open
        //     method has not been called or the System.IO.Ports.SerialPort.Close method has
        //     been called.
        public bool CDHolding { get; }
        //
        // Summary:
        //     Gets or sets the number of milliseconds before a time-out occurs when a read
        //     operation does not finish.
        //
        // Returns:
        //     The number of milliseconds before a time-out occurs when a read operation does
        //     not finish.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The read time-out value is less than zero and not equal to System.IO.Ports.SerialPort.InfiniteTimeout.
        public int ReadTimeout { get; set; }
        //
        // Summary:
        //     Gets or sets the number of bytes in the internal input buffer before a System.IO.Ports.SerialPort.DataReceived
        //     event occurs.
        //
        // Returns:
        //     The number of bytes in the internal input buffer before a System.IO.Ports.SerialPort.DataReceived
        //     event is fired. The default is 1.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The System.IO.Ports.SerialPort.ReceivedBytesThreshold value is less than or equal
        //     to zero.
        public int ReceivedBytesThreshold { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether the Request to Send (RTS) signal is enabled
        //     during serial communication.
        //
        // Returns:
        //     true to enable Request to Transmit (RTS); otherwise, false. The default is false.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The value of the System.IO.Ports.SerialPort.RtsEnable property was set or retrieved
        //     while the System.IO.Ports.SerialPort.Handshake property is set to the System.IO.Ports.Handshake.RequestToSend
        //     value or the System.IO.Ports.Handshake.RequestToSendXOnXOff value.
        //
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        public bool RtsEnable { get; set; }
        //
        // Summary:
        //     Gets or sets the standard number of stopbits per byte.
        //
        // Returns:
        //     One of the System.IO.Ports.StopBits values.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The System.IO.Ports.SerialPort.StopBits value is System.IO.Ports.StopBits.None.
        //
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        public StopBits StopBits { get; set; }
        //
        // Summary:
        //     Gets or sets the size of the serial port output buffer.
        //
        // Returns:
        //     The size of the output buffer. The default is 2048.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The System.IO.Ports.SerialPort.WriteBufferSize value is less than or equal to
        //     zero.
        //
        //   T:System.InvalidOperationException:
        //     The System.IO.Ports.SerialPort.WriteBufferSize property was set while the stream
        //     was open.
        //
        //   T:System.IO.IOException:
        //     The System.IO.Ports.SerialPort.WriteBufferSize property was set to an odd integer
        //     value.
        public int WriteBufferSize { get; set; }
        //
        // Summary:
        //     Gets or sets the number of milliseconds before a time-out occurs when a write
        //     operation does not finish.
        //
        // Returns:
        //     The number of milliseconds before a time-out occurs. The default is System.IO.Ports.SerialPort.InfiniteTimeout.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The System.IO.Ports.SerialPort.WriteTimeout value is less than zero and not equal
        //     to System.IO.Ports.SerialPort.InfiniteTimeout.
        public int WriteTimeout { get; set; }
        //
        // Summary:
        //     Gets or sets the parity-checking protocol.
        //
        // Returns:
        //     One of the enumeration values that represents the parity-checking protocol. The
        //     default is System.IO.Ports.Parity.None.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The System.IO.Ports.SerialPort.Parity value passed is not a valid value in the
        //     System.IO.Ports.Parity enumeration.
        public Parity Parity { get; set; }
        //
        // Summary:
        //     Gets the number of bytes of data in the send buffer.
        //
        // Returns:
        //     The number of bytes of data in the send buffer.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state.
        //
        //   T:System.InvalidOperationException:
        //     The stream is closed. This can occur because the System.IO.Ports.SerialPort.Open
        //     method has not been called or the System.IO.Ports.SerialPort.Close method has
        //     been called.
        public int BytesToWrite { get; }
        //
        // Summary:
        //     Gets or sets the serial baud rate.
        //
        // Returns:
        //     The baud rate.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     The baud rate specified is less than or equal to zero, or is greater than the
        //     maximum allowable baud rate for the device.
        //
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        public int BaudRate { get; set; }
        //
        // Summary:
        //     Gets or sets the break signal state.
        //
        // Returns:
        //     true if the port is in a break state; otherwise, false.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.InvalidOperationException:
        //     The stream is closed. This can occur because the System.IO.Ports.SerialPort.Open
        //     method has not been called or the System.IO.Ports.SerialPort.Close method has
        //     been called.
        public bool BreakState { get; set; }
        //
        // Summary:
        //     Gets the underlying System.IO.Stream object for a System.IO.Ports.SerialPort
        //     object.
        //
        // Returns:
        //     A System.IO.Stream object.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The stream is closed. This can occur because the System.IO.Ports.SerialPort.Open
        //     method has not been called or the System.IO.Ports.SerialPort.Close method has
        //     been called.
        //
        //   T:System.NotSupportedException:
        //     The stream is in a .NET Compact Framework application and one of the following
        //     methods was called: System.IO.Stream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)System.IO.Stream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)System.IO.Stream.EndRead(System.IAsyncResult)System.IO.Stream.EndWrite(System.IAsyncResult)
        //     The .NET Compact Framework does not support the asynchronous model with base
        //     streams.
        public Stream BaseStream { get; }
        //
        // Summary:
        //     Gets the number of bytes of data in the receive buffer.
        //
        // Returns:
        //     The number of bytes of data in the receive buffer.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The port is not open.
        public int BytesToRead { get; }

        //
        // Summary:
        //     Indicates that data has been received through a port represented by the System.IO.Ports.SerialPort
        //     object.
        public event SerialDataReceivedEventHandler DataReceived;
        //
        // Summary:
        //     Indicates that a non-data signal event has occurred on the port represented by
        //     the System.IO.Ports.SerialPort object.
        public event SerialPinChangedEventHandler PinChanged;
        //
        // Summary:
        //     Indicates that an error has occurred with a port represented by a System.IO.Ports.SerialPort
        //     object.
        public event SerialErrorReceivedEventHandler ErrorReceived;

        //
        // Summary:
        //     Closes the port connection, sets the System.IO.Ports.SerialPort.IsOpen property
        //     to false, and disposes of the internal System.IO.Stream object.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        public void Close();
        //
        // Summary:
        //     Discards data from the serial driver's receive buffer.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.InvalidOperationException:
        //     The stream is closed. This can occur because the System.IO.Ports.SerialPort.Open
        //     method has not been called or the System.IO.Ports.SerialPort.Close method has
        //     been called.
        public void DiscardInBuffer();
        //
        // Summary:
        //     Discards data from the serial driver's transmit buffer.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.InvalidOperationException:
        //     The stream is closed. This can occur because the System.IO.Ports.SerialPort.Open
        //     method has not been called or the System.IO.Ports.SerialPort.Close method has
        //     been called.
        public void DiscardOutBuffer();
        //
        // Summary:
        //     Opens a new serial port connection.
        //
        // Exceptions:
        //   T:System.UnauthorizedAccessException:
        //     Access is denied to the port. -or- The current process, or another process on
        //     the system, already has the specified COM port open either by a System.IO.Ports.SerialPort
        //     instance or in unmanaged code.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     One or more of the properties for this instance are invalid. For example, the
        //     System.IO.Ports.SerialPort.Parity, System.IO.Ports.SerialPort.DataBits, or System.IO.Ports.SerialPort.Handshake
        //     properties are not valid values; the System.IO.Ports.SerialPort.BaudRate is less
        //     than or equal to zero; the System.IO.Ports.SerialPort.ReadTimeout or System.IO.Ports.SerialPort.WriteTimeout
        //     property is less than zero and is not System.IO.Ports.SerialPort.InfiniteTimeout.
        //
        //   T:System.ArgumentException:
        //     The port name does not begin with "COM". -or- The file type of the port is not
        //     supported.
        //
        //   T:System.IO.IOException:
        //     The port is in an invalid state. -or- An attempt to set the state of the underlying
        //     port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   T:System.InvalidOperationException:
        //     The specified port on the current instance of the System.IO.Ports.SerialPort
        //     is already open.
        public void Open();
        //
        // Summary:
        //     Reads a number of bytes from the System.IO.Ports.SerialPort input buffer and
        //     writes those bytes into a byte array at the specified offset.
        //
        // Parameters:
        //   buffer:
        //     The byte array to write the input to.
        //
        //   offset:
        //     The offset in buffer at which to write the bytes.
        //
        //   count:
        //     The maximum number of bytes to read. Fewer bytes are read if count is greater
        //     than the number of bytes in the input buffer.
        //
        // Returns:
        //     The number of bytes read.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The buffer passed is null.
        //
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The offset or count parameters are outside a valid region of the buffer being
        //     passed. Either offset or count is less than zero.
        //
        //   T:System.ArgumentException:
        //     offset plus count is greater than the length of the buffer.
        //
        //   T:System.TimeoutException:
        //     No bytes were available to read.
        public int Read(byte[] buffer, int offset, int count);
        //
        // Summary:
        //     Reads a number of characters from the System.IO.Ports.SerialPort input buffer
        //     and writes them into an array of characters at a given offset.
        //
        // Parameters:
        //   buffer:
        //     The character array to write the input to.
        //
        //   offset:
        //     The offset in buffer at which to write the characters.
        //
        //   count:
        //     The maximum number of characters to read. Fewer characters are read if count
        //     is greater than the number of characters in the input buffer.
        //
        // Returns:
        //     The number of characters read.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     offset plus count is greater than the length of the buffer. -or- count is 1 and
        //     there is a surrogate character in the buffer.
        //
        //   T:System.ArgumentNullException:
        //     The buffer passed is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The offset or count parameters are outside a valid region of the buffer being
        //     passed. Either offset or count is less than zero.
        //
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.TimeoutException:
        //     No characters were available to read.
        public int Read(char[] buffer, int offset, int count);
        //
        // Summary:
        //     Synchronously reads one byte from the System.IO.Ports.SerialPort input buffer.
        //
        // Returns:
        //     The byte, cast to an System.Int32, or -1 if the end of the stream has been read.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.ServiceProcess.TimeoutException:
        //     The operation did not complete before the time-out period ended. -or- No byte
        //     was read.
        public int ReadByte();
        //
        // Summary:
        //     Synchronously reads one character from the System.IO.Ports.SerialPort input buffer.
        //
        // Returns:
        //     The character that was read.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.ServiceProcess.TimeoutException:
        //     The operation did not complete before the time-out period ended. -or- No character
        //     was available in the allotted time-out period.
        public int ReadChar();
        //
        // Summary:
        //     Reads all immediately available bytes, based on the encoding, in both the stream
        //     and the input buffer of the System.IO.Ports.SerialPort object.
        //
        // Returns:
        //     The contents of the stream and the input buffer of the System.IO.Ports.SerialPort
        //     object.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        public string ReadExisting();
        //
        // Summary:
        //     Reads up to the System.IO.Ports.SerialPort.NewLine value in the input buffer.
        //
        // Returns:
        //     The contents of the input buffer up to the first occurrence of a System.IO.Ports.SerialPort.NewLine
        //     value.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.TimeoutException:
        //     The operation did not complete before the time-out period ended. -or- No bytes
        //     were read.
        public string ReadLine();
        //
        // Summary:
        //     Reads a string up to the specified value in the input buffer.
        //
        // Parameters:
        //   value:
        //     A value that indicates where the read operation stops.
        //
        // Returns:
        //     The contents of the input buffer up to the specified value.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The length of the value parameter is 0.
        //
        //   T:System.ArgumentNullException:
        //     The value parameter is null.
        //
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.TimeoutException:
        //     The operation did not complete before the time-out period ended.
        public string ReadTo(string value);
        //
        // Summary:
        //     Writes a specified number of bytes to the serial port using data from a buffer.
        //
        // Parameters:
        //   buffer:
        //     The byte array that contains the data to write to the port.
        //
        //   offset:
        //     The zero-based byte offset in the buffer parameter at which to begin copying
        //     bytes to the port.
        //
        //   count:
        //     The number of bytes to write.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The buffer passed is null.
        //
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The offset or count parameters are outside a valid region of the buffer being
        //     passed. Either offset or count is less than zero.
        //
        //   T:System.ArgumentException:
        //     offset plus count is greater than the length of the buffer.
        //
        //   T:System.ServiceProcess.TimeoutException:
        //     The operation did not complete before the time-out period ended.
        public void Write(byte[] buffer, int offset, int count);
        //
        // Summary:
        //     Writes the specified string to the serial port.
        //
        // Parameters:
        //   text:
        //     The string for output.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.ArgumentNullException:
        //     text is null.
        //
        //   T:System.ServiceProcess.TimeoutException:
        //     The operation did not complete before the time-out period ended.
        public void Write(string text);
        //
        // Summary:
        //     Writes a specified number of characters to the serial port using data from a
        //     buffer.
        //
        // Parameters:
        //   buffer:
        //     The character array that contains the data to write to the port.
        //
        //   offset:
        //     The zero-based byte offset in the buffer parameter at which to begin copying
        //     bytes to the port.
        //
        //   count:
        //     The number of characters to write.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The buffer passed is null.
        //
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     The offset or count parameters are outside a valid region of the buffer being
        //     passed. Either offset or count is less than zero.
        //
        //   T:System.ArgumentException:
        //     offset plus count is greater than the length of the buffer.
        //
        //   T:System.ServiceProcess.TimeoutException:
        //     The operation did not complete before the time-out period ended.
        public void Write(char[] buffer, int offset, int count);
        //
        // Summary:
        //     Writes the specified string and the System.IO.Ports.SerialPort.NewLine value
        //     to the output buffer.
        //
        // Parameters:
        //   text:
        //     The string to write to the output buffer.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The text parameter is null.
        //
        //   T:System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   T:System.TimeoutException:
        //     The System.IO.Ports.SerialPort.WriteLine(System.String) method could not write
        //     to the stream.
        public void WriteLine(string text);
    }
}
