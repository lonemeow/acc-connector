param($ver, $file)

$major, $minor = $ver -split "\."

@"
#define VERSION_BIN $major,$minor,0,0
#define VERSION_STR "$major.$minor.0.0"
"@ | Out-File -Encoding ASCII $file
