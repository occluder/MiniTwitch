# MiniTwitch.Common Changelog

## 2.0.1

### Minor changes
- Updated the default logger to print scopes.
***

## 2.0.0
This release marks the removal of .NET 6 and .NET 7 as target frameworks, as both versions are out-of-support by now.
***

## 1.1.10

### Fixes

- Fixed WebSocketClient swallowing exceptions when resolving a uri fails

***

## 1.1.5

### Minor changes

- Improved handling of WebSocket reads

### Dev

- Added `ReadOnlySpan<byte>.MSum()` extension, which returns sum * first value