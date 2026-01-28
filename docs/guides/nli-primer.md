---
layout: default
title: What is NLI?
parent: Guides
nav_order: 5
permalink: /guides/nli-primer
---

# Native Library Interop: The Friendly Neighborhood Translator

![Native Library Interop]({{ site.baseurl }}/images/maui-nli-binding.png)

## What is NLI?

**Native Library Interop (NLI)** is like having Spider-Man swing between two buildings - except the buildings are programming languages, and Spider-Man is your C# code trying to talk to native mobile libraries.

When you write a .NET MAUI app, you're writing C#. But the really cool mobile SDKs (like Datadog's) are written in the native languages:

- **Android**: Kotlin/Java
- **iOS**: Swift/Objective-C

NLI is the bridge that lets your C# code call into these native libraries without you having to learn Kotlin, Java, Swift, or Objective-C. It's basically a universal translator for code.

## How Does It Work?

Think of it like ordering at a restaurant in a foreign country:

1. **You** (C# developer) want to order food
2. **The waiter** (NLI binding) speaks both English and the local language
3. **The chef** (native SDK) only speaks the local language
4. The waiter translates your order → chef cooks → waiter translates back with your food

In code terms:

```csharp
// You write this in C#:
Datadog.Initialize(config);

// The binding translates it to:
// Android: DatadogSdk.initialize(context, configuration)
// iOS: Datadog.initialize(configuration: config)

// You get the result back in C# - no translation needed!
```

## Why Do We Need Bindings?

Mobile platforms have **decades** of mature, battle-tested native libraries. Instead of rewriting everything in C#, we create **bindings** - thin wrappers that expose native functionality through C# APIs.

### For Android (Java/Kotlin → C#)

- We use **.NET for Android bindings** (formerly Xamarin.Android)
- Takes Java/Kotlin `.jar` or `.aar` files
- Generates C# classes that call the native code

### For iOS (Swift/Objective-C → C#)

- We use **.NET for iOS bindings** (formerly Xamarin.iOS)
- Takes Objective-C `.framework` or Swift modules
- Generates C# classes that call the native code

## The Secret Sauce

Here's where it gets fun: when you call a C# method in a binding, under the hood it's doing something like this:

**Android:**

```csharp
// Your C# call
datadog.TrackEvent("user_login");

// Becomes a JNI (Java Native Interface) call
JNIEnv.CallVoidMethod(javaObject, methodId, javaArgs);
```

**iOS:**

```csharp
// Your C# call
datadog.TrackEvent("user_login");

// Becomes an Objective-C runtime call
objc_msgSend(nativeObject, selector, args);
```

You never see this complexity - the binding handles it all!

## Why This Matters for Datadog MAUI SDK

The Datadog MAUI SDK is actually **three layers** working together:

1. **Native SDKs** - The real heavy lifters (Kotlin/Swift)
   - Datadog's Android SDK (written in Kotlin)
   - Datadog's iOS SDK (written in Swift)

2. **Platform Bindings** - The translators (C# → Native)
   - `Datadog.MAUI.Android.*` packages
   - `Datadog.MAUI.iOS.*` packages

3. **Unified Plugin** - The friendly interface (Pure C#)
   - `Datadog.MAUI` package
   - Single API that works on both platforms

```
Your App (C#)
    ↓
Unified Plugin (C# - cross-platform)
    ↓
Platform Bindings (C# - platform-specific)
    ↓
Native SDKs (Kotlin/Swift)
    ↓
Datadog Backend
```

## The Good News

As a user of the SDK, you don't need to think about any of this! You just:

```csharp
// Install the package
dotnet add package Datadog.MAUI

// Use the unified API
Datadog.Initialize(config);
Datadog.Logs.Info("Hello from C#!");
```

The bindings handle all the translation magic behind the scenes.

## The "Why Not Pure C#?" Question

Fair question! Why not just rewrite everything in C#?

**Short answer:** We'd rather spend our time building features than rewriting perfectly good code.

**Long answer:**

- Native SDKs are **massive** (thousands of lines of battle-tested code)
- They're updated **frequently** with bug fixes and new features
- They're optimized for their specific platforms
- By using bindings, we get all native updates automatically
- Our job is just to keep the translation layer up-to-date

## Learn More

Want to dive deeper into how we build these bindings?

- **Android**: See [Android Dependencies](/guides/android/dependencies)
- **iOS**: See [iOS Binding Strategy](/guides/ios/binding-strategy)
- **Architecture**: See [Packaging Architecture](/architecture/packaging)

---

_Now you know the secret: when you call a C# method, it's actually a superhero swinging between language worlds to make it work!_ 🕷️
