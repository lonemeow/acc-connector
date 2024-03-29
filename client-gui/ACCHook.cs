﻿using Microsoft.Win32;
using System.Diagnostics;
using System.Text;
using Win32;

namespace ACCConnector {

    public class ACCHook {

        private static readonly string[][] HOOK_DLL_LOCATIONS = [
            ["client-hooks.dll"],
            ["..", "..", "..", "..", "x64", "Debug", "client-hooks.dll"],
            ["..", "..", "..", "..", "x64", "Release", "client-hooks.dll"]];

        public static string FindHookDLL() {
            var exeLocation = Path.GetDirectoryName(Path.GetFullPath(Application.ExecutablePath));
            foreach (var loc in HOOK_DLL_LOCATIONS) {
                var dllLocation = Path.GetFullPath(Path.Combine([exeLocation!, .. loc]));
                Trace.WriteLine($"Looking for hook DLL in {dllLocation}");
                if (File.Exists(dllLocation)) {
                    return dllLocation;
                }
            }
            throw new Exception("Hook DLL not found");
        }

        private static bool IsACCWindow(IntPtr hWnd) {
            char[] buffer = new char[128];
            if (User32.GetWindowText(hWnd, buffer, buffer.Length) == 0) {
                return false;
            }
            var windowTitle = NullTerminatedString(buffer);
            if (User32.GetClassName(hWnd, buffer, buffer.Length) == 0) {
                throw new APIFailureException("User32.GetClassName");
            }
            var windowClass = NullTerminatedString(buffer);

            return windowTitle == "AC2  " && windowClass == "UnrealWindow";
        }

        public static bool IsACCRunning() {
            bool found = false;
            if (!User32.EnumWindows(delegate (IntPtr hWnd, IntPtr lParam) {
                if (IsACCWindow(hWnd)) {
                    Trace.WriteLine("ACC window found");
                    found = true;
                }
                return true;
            }, IntPtr.Zero)) {
                throw new APIFailureException("User32.EnumWindows");
            }
            return found;
        }

        private static string NullTerminatedString(char[] buffer) {
            return new string(buffer.TakeWhile(c => c != '\0').ToArray());
        }

        private static IEnumerable<string> FindSteamFolders() {
            using var steamSubKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Valve\\Steam");
            if (steamSubKey == null || steamSubKey.GetValue("InstallPath") is not string steamInstallPath) {
                Trace.WriteLine("Steam install path not found in registry");
                return Enumerable.Empty<string>();
            }
            Trace.WriteLine($"Steam installed in {steamInstallPath}");

            using var steamFolderReader = new StreamReader(Path.Join(steamInstallPath, "steamapps", "libraryfolders.vdf"), Encoding.UTF8);
            if (VDFSerializer.Deserialize(steamFolderReader).Item2 is not Dictionary<string, object> folderInfo) {
                Trace.WriteLine("Could not parse Steam library folders");
                return Enumerable.Empty<string>();
            }
            var steamFolders = from Dictionary<string, object> dict in folderInfo.Values
                               select dict["path"] as string;
            Trace.WriteLine($"Steam library folders: {String.Join(", ", steamFolders)}");
            return steamFolders;
        }

        public static string? FindACCInstallDir() {
            foreach (var f in FindSteamFolders()) {
                var accManifestPath = Path.Join(f, "steamapps", "appmanifest_805550.acf");
                if (File.Exists(accManifestPath)) {
                    Trace.WriteLine($"ACC appmanifest found in {accManifestPath}");
                    using var acfReader = new StreamReader(accManifestPath, Encoding.UTF8);
                    if (VDFSerializer.Deserialize(acfReader).Item2 is not Dictionary<string, object> accInfo) {
                        Trace.WriteLine("Could not parse ACC appmanifest");
                        break;
                    }
                    return Path.Join(f, "steamapps", "common", (string)accInfo["installdir"]);
                }
            }
            return null;
        }

        public static bool IsHookInstalled(string accInstallPath) {
            return File.Exists(InstallPathToDllPath(accInstallPath));
        }

        public static bool IsHookOutdated(string accInstallPath) {
            if (IsHookInstalled(accInstallPath)) {
                var myInfo = FileVersionInfo.GetVersionInfo(FindHookDLL());
                var verInfo = FileVersionInfo.GetVersionInfo(InstallPathToDllPath(accInstallPath));
                Trace.WriteLine($"Installed hook version: {verInfo.ProductVersion} my version {myInfo.ProductVersion}");
                return myInfo.ProductVersion != verInfo.ProductVersion;
            }
            return false;
        }

        private static string InstallPathToDllPath(string accInstallPath) {
            return Path.Join(accInstallPath, "AC2", "Binaries", "Win64", "hid.dll");
        }

        public static void RemoveHook(string accInstallPath) {
            var dllPath = InstallPathToDllPath(accInstallPath);
            File.Delete(InstallPathToDllPath(accInstallPath));
            Trace.WriteLine($"Deleted hook DLL from {dllPath}");
        }

        public static void InstallHook(string accInstallPath) {
            var dllPath = InstallPathToDllPath(accInstallPath);
            File.Copy(FindHookDLL(), dllPath, true);
            Trace.WriteLine($"Copied hook DLL to {dllPath}");
        }
    }
}
