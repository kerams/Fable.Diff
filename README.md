# Fable.Diff

Fable bindings for [jsdiff/diff](https://github.com/kpdecker/jsdiff) ([NPM package](https://www.npmjs.com/package/diff)) version 5.0.0+.

## Nuget package
[![Nuget](https://img.shields.io/nuget/v/Fable.Diff.svg?colorB=green)](https://www.nuget.org/packages/Fable.Diff)

## Installation with [Femto](https://github.com/Zaid-Ajaj/Femto)

```
femto install Fable.Diff
```

## Standard installation

Nuget package

```
paket add Fable.Diff -p YourProject.fsproj
```

NPM package

```
npm install diff@5.0.0
```

## Usage

Use the `Diff` object to call any function of the [public API](https://github.com/kpdecker/jsdiff#api).

```fsharp
open Fable.Diff
open Fable.React
open Fable.React.Props

let viewChanges () =
    let oldText = "beep boop"
    let newText = "beep boob blah"

    // Get a char-by-char diff of two strings. The return value here is an array with 3 elements representing
    // the changed text parts between the 2 strings. You can also use the optional parameter to toggle case sensitivity.
    // let diff = Diff.diffChars (oldText, newText, Fable.Core.JsInterop.jsOptions<IBaseOptions> (fun x -> x.ignoreCase <- true))
    let diff = Diff.diffChars (oldText, newText)

    div [ Style [ BackgroundColor "black" ] ] [
        for part in diff do
            // Green for additions, red for deletions, grey for common parts.
            let color =
                if part.added then
                    "green"
                elif part.removed then
                    "red"
                else
                    "grey"

            span [ Style [ Color color ] ] [ str part.value ]
    ]
```

If you were to render the output of the function, you'd see something like this:

![](https://raw.githubusercontent.com/kpdecker/jsdiff/HEAD/images/node_example.png)