
If you won't read the `version.txt` file automatically , set `DisableAutoVersionFileRead` to `true`

You access the `UtopiaSdkVersion` MSBuild property,it's same with the version of the SDK and `Utopia.Build`,`Kawayi.Utopia.Sdk` packages.

If you want to change the `version.txt` file path of name, set `UtopiaVersionFilePath` to you want.

Set `UseUtopiaBuild` to `false` to remove the `<PackageReference Include="Utopia.Build" Version="$(UtopiaSdkVersion)" />`
Set `UseUtopiaSdk` to `false` to remove the `<PackageReference Include="Kawayi.Utopia.Sdk" Version="$(UtopiaSdkVersion)" />`
