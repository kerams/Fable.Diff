module Fable.Diff

open System
open Fable.Core

type IChange =
    /// Text content.
    abstract value: string

    /// `true` if the value was inserted into the new string.
    abstract added: bool

    /// `true` if the value was removed from the old string.
    abstract removed: bool

type IArrayChange<'a> =
    abstract value: 'a[]
    abstract count: int
    abstract added: bool
    abstract removed: bool

type IHunk =
    abstract oldStart: int
    abstract oldLines: int
    abstract newStart: int
    abstract newLines: int
    abstract lines: string[]
    abstract linedelimiters: string[]

type IParsedDiff =
    abstract index: string
    abstract oldFileName: string
    abstract newFileName: string
    abstract oldHeader: string
    abstract newHeader: string
    abstract hunks: IHunk[]

type IBaseOptions =
    /// `true` to ignore casing difference.
    abstract ignoreCase: bool with get, set

type ILinesOptions =
    inherit IBaseOptions

    /// `true` to ignore leading and trailing whitespace. This is the same as `diffTrimmedLines()`.
    abstract ignoreWhitespace: bool with get, set

    /// `true` to treat newline characters as separate tokens. This allows for changes to the newline structure
    /// to occur independently of the line content and to be treated as such. In general this is the more
    /// human friendly form of `diffLines()` and `diffLines()` is better suited for patches and other computer
    /// friendly output.
    abstract newlineIsToken: bool with get, set

type IJsonOptions =
    inherit ILinesOptions

    /// Replacer used to stringify the properties of the passed objects.
    abstract stringifyReplacer: Func<string, obj, obj> with get, set

    /// The value to use when `undefined` values in the passed objects are encountered during stringification.
    /// Will only be used if `stringifyReplacer` option wasn't specified.
    abstract undefinedReplacement: obj with get, set

type IArrayOptions<'a> =
    abstract comparator: Func<'a, 'a, bool> with get, set        

type IPatchOptions =
    inherit ILinesOptions

    /// Describes how many lines of context should be included. Default 4.
    abstract context: int with get, set

type IApplyPatchOptions =
    /// Number of lines that are allowed to differ before rejecting a patch. Default 0.
    abstract fuzzFactor: int with get, set

    /// Callback used to compare to given lines to determine if they should be considered equal when patching.
    /// Should return `false` if the lines should be rejected.
    /// The parameters are: lineNumber, line, operation ('-' or ' '), patchContent.
    abstract compareLine: Func<int, string, string, string, bool> with get, set

type IApplyPatchesOptions =
    inherit IApplyPatchOptions

    abstract loadFile: index: IParsedDiff * callback: Action<obj, string> -> unit
    abstract patched: index: IParsedDiff * content: string * callback: (obj -> unit) -> unit
    abstract complete: err: obj -> unit

type IParsePatchOptions =
    abstract strict: bool with get, set

type IDiff =
    /// <summary>
    /// Diffs two blocks of text, comparing character by character.
    /// </summary>
    /// <returns>A list of change objects.</returns>
    abstract diffChars: oldStr: string * newStr: string * ?options: IBaseOptions -> IChange[]

    /// <summary>
    /// Diffs two blocks of text, comparing word by word, ignoring whitespace.
    /// </summary>
    /// <returns>A list of change objects.</returns>
    abstract diffWords: oldStr: string * newStr: string * ?options: IBaseOptions -> IChange[]

    /// <summary>
    /// Diffs two blocks of text, comparing word by word, treating whitespace as significant.
    /// </summary>
    /// <returns>A list of change objects.</returns>
    abstract diffWordsWithSpace: oldStr: string * newStr: string * ?options: IBaseOptions -> IChange[]

    /// <summary>
    /// Diffs two blocks of text, comparing line by line.
    /// </summary>
    /// <returns>A list of change objects.</returns>
    abstract diffLines: oldStr: string * newStr: string * ?options: ILinesOptions -> IChange[]

    /// <summary>
    /// Diffs two blocks of text, comparing line by line.
    /// </summary>
    /// <returns>A list of change objects.</returns>
    abstract diffTrimmedLines: oldStr: string * newStr: string * ?options: ILinesOptions -> IChange[]

    /// <summary>
    /// Diffs two blocks of text, comparing sentence by sentence.
    /// </summary>
    /// <returns>A list of change objects.</returns>
    abstract diffSentences: oldStr: string * newStr: string * ?options: IBaseOptions -> IChange[]

    /// <summary>
    /// Diffs two blocks of text, comparing CSS tokens.
    /// </summary>
    /// <returns>A list of change objects.</returns>
    abstract diffCss: oldStr: string * newStr: string * ?options: IBaseOptions -> IChange[]

    /// <summary>
    /// Diffs two JSON objects, comparing the fields defined on each. The order of fields, etc does not matter in this comparison.
    /// </summary>
    /// <returns>A list of change objects.</returns>
    abstract diffJson: oldObj: U2<string, obj> * newObj: U2<string, obj> * ?options: IJsonOptions -> IChange[]

    /// <summary>
    /// Diffs two arrays, comparing each item for strict equality (`===`).
    /// </summary>
    /// <returns>A list of change objects.</returns>
    abstract diffArrays: oldArr: 'a[] * newArr: 'a[] * ?options: IArrayOptions<'a> -> IArrayChange<'a>[]

    /// <summary>
    /// Creates a unified diff patch.
    /// </summary>
    /// <param name="oldFileName">String to be output in the filename section of the patch for the removals.</param>
    /// <param name="newFileName">String to be output in the filename section of the patch for the additions.</param>
    /// <param name="oldStr">Original string value.</param>
    /// <param name="newStr">New string value.</param>
    /// <param name="oldHeader">Additional information to include in the old file header.</param>
    /// <param name="newHeader">Additional information to include in the new file header.</param>
    abstract createTwoFilesPatch: oldFileName: string * newFileName: string * oldStr: string * newStr: string * ?oldHeader: string * ?newHeader: string * ?options: IPatchOptions -> string

    /// <summary>
    /// Creates a unified diff patch. Just like `createTwoFilesPatch()`, but with `oldFileName` being equal to `newFileName`.
    /// </summary>
    /// <param name="fileName">String to be output in the filename section.</param>
    /// <param name="oldStr">Original string value.</param>
    /// <param name="newStr">New string value.</param>
    /// <param name="oldHeader">Additional information to include in the old file header.</param>
    /// <param name="newHeader">Additional information to include in the new file header.</param>
    abstract createPatch: oldFileName: string * newFileName: string * oldStr: string * newStr: string * ?oldHeader: string * ?newHeader: string * ?options: IPatchOptions -> string

    /// <summary>
    /// This method is similar to `createTwoFilesPatch()`, but returns a data structure suitable for further processing.
    /// </summary>
    /// <param name="oldFileName">String to be output in the `oldFileName` hunk property.</param>
    /// <param name="newFileName">String to be output in the `newFileName` hunk property.</param>
    /// <param name="oldStr">Original string value.</param>
    /// <param name="newStr">New string value.</param>
    /// <param name="oldHeader">Additional information to include in the `oldHeader` hunk property.</param>
    /// <param name="newHeader">Additional information to include in the `newHeader` hunk property.</param>
    /// <returns>An object with an array of hunk objects.</returns>
    abstract structuredPatch: oldFileName: string * newFileName: string * oldStr: string * newStr: string * ?oldHeader: string * ?newHeader: string * ?options: IPatchOptions -> IParsedDiff

    /// <summary>
    /// Applies a unified diff patch.
    /// </summary>
    /// <param name="patch">May be a string diff or the output from the `parsePatch()` or `structuredPatch()` methods.</param>
    /// <returns>A string containing new version of provided data.</returns>
    abstract applyPatch: source: string * patch: U2<string, IParsedDiff> * ?options: IApplyPatchOptions -> string

    /// <summary>
    /// Applies one or more patches.
    ///
    /// This method will iterate over the contents of the patch and apply to data provided through callbacks.
    ///
    /// The general flow for each patch index is:
    ///
    /// 1. `options.loadFile(index, callback)` is called. The caller should then load the contents of the file
    /// and then pass that to the `callback(err, data)` callback. Passing an `err` will terminate further patch execution.
    /// 
    /// 2. `options.patched(index, content, callback)` is called once the patch has been applied. `content` will be
    /// the return value from `applyPatch()`. When it's ready, the caller should call `callback(err)` callback.
    /// Passing an `err` will terminate further patch execution.
    /// 
    /// 3. Once all patches have been applied or an error occurs, the `options.complete(err)` callback is made.
    /// </summary>
    abstract applyPatches: patch: U2<string, IParsedDiff[]> * options: IApplyPatchesOptions -> unit

    /// <summary>
    /// Parses a patch into structured data.
    /// </summary>
    /// <returns>A JSON object representation of the a patch, suitable for use with the `applyPatch()` method.</returns>
    abstract parsePatch: diffStr: string * ?options: IParsePatchOptions -> IParsedDiff[]

    /// <summary>
    /// Converts a list of changes to a serialized XML format.
    /// </summary>
    abstract convertChangesToXML: changes: IChange[] -> string

    abstract merge: mine: string * theirs: string * ``base``: string -> IParsedDiff

[<ImportAll("diff")>]
let Diff = jsNative<IDiff>