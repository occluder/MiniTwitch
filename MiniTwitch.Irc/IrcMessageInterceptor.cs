namespace MiniTwitch.Irc;

/// <summary>
/// Provides a base class for intercepting IRC message data before parsing.
/// </summary>
/// <remarks>Derive from this class to implement custom logic for inspecting incoming IRC messages prior to parsing.
/// <para>This class is intended for advanced scenarios such as filtering or logging IRC messages messages.</para>
/// </remarks>
public abstract class IrcMessageInterceptor
{
    /// <summary>
    /// Determines whether parsing should proceed for the specified message.
    /// </summary>
    /// <remarks>
    /// Override this method to implement custom logic for filtering or validating messages prior to parsing. 
    /// The default implementation always returns true, allowing all messages to be parsed.
    /// </remarks>
    /// <param name="source">The <see cref="IrcClient"/> calling this method.</param>
    /// <param name="message">A read-only span of bytes representing the message to be evaluated before parsing.</param>
    /// <returns><see langword="true"/> if parsing should continue for the provided message; otherwise, <see langword="false"/>.</returns>
    public virtual bool BeforeParse(IrcClient source, ReadOnlySpan<byte> message)
    {
        return true;
    }

    /// <summary>
    /// Provides a hook for performing additional processing after a message has been parsed.
    /// </summary>
    /// <remarks>
    /// Override this method to implement custom post-processing logic after parsing a message. 
    /// This is called before the event is sent. 
    /// The default implementation always returns true, allowing all events to be sent.
    /// </remarks>
    /// <typeparam name="T">The type of the parsed result produced from the message.</typeparam>
    /// <param name="source">The <see cref="IrcClient"/> calling this method.</param>
    /// <param name="result">The parsed result object generated from the message.</param>
    /// <param name="message">The original message data as a read-only span of bytes.</param>
    /// <returns><see langword="true"/> if the event should be sent; otherwise, <see langword="false"/>.</returns>
    public virtual bool AfterParse<T>(IrcClient source, T result, ReadOnlySpan<byte> message)
    {
        return true;
    }
}