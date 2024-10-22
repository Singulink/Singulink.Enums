namespace System.Buffers;

#if NETSTANDARD

internal delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);

#endif
