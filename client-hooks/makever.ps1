param($ver, $file)

$major, $minor, $patch = $ver -split "\."

@"
#define VERSION_BIN $major,$minor,$patch,0
#define VERSION_STR "$major.$minor.$patch.0"
"@ | Out-File -Encoding ASCII $file
