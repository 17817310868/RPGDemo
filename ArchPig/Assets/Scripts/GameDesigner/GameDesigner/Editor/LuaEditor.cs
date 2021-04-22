using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;

namespace Lua
{
    public class LuaEditor : EditorWindow
    {
        static LuaEditor edit;
        private string path, path1;
        static FileSystemWatcher watcher;
        static bool changed = false;
        static List<string> modifyFiles = new List<string>();
        bool isPlay;

        [MenuItem("Lua/LuaWindow")]
        public static void Init()
        {
            edit = GetWindow<LuaEditor>("LuaEdit");
        }

        void Changed(string path)
        {
            if (watcher != null)
                watcher.Dispose();
            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.cs";
            watcher.Changed += (send, args1) => {
                modifyFiles.Add(args1.FullPath);
                changed = true;
            };
            watcher.EnableRaisingEvents = true;
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            path = GUILayout.TextField(path);
            if (GUILayout.Button("选择镜像文件夹", GUILayout.Width(100))) {
                path = EditorUtility.OpenFolderPanel("C#源码文件夹", "", "");
                Changed(path.Replace("/", "\\") + "\\");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            path1 = GUILayout.TextField(path1);
            if (GUILayout.Button("选择资源文件夹", GUILayout.Width(100))) {
                path1 = EditorUtility.OpenFolderPanel("热更新cs.txt文件", "", "");
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("更新文件", GUILayout.Height(50))) {
                List<string> modifyFiles = new List<string>();
                var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
                foreach (var file in files) {
                    var bytes = File.ReadAllBytes(file);
                    var file1 = path1 + "/" + Path.GetFileNameWithoutExtension(file) + ".txt";
                    if (File.Exists(file1)) {
                        FileInfo info = new FileInfo(file1);
                        if (info.Length == bytes.Length)
                            continue;
                        File.WriteAllBytes(file1, bytes);
                    } else {
                        File.WriteAllBytes(file1, bytes);
                    }
                    modifyFiles.Add(Path.GetFileNameWithoutExtension(file));
                }

                Debug.Log("更新成功!");
                AssetDatabase.Refresh();

                var luas = FindObjectsOfType<LuaComponent>();
                foreach (var file in modifyFiles) {
                    var file1 = Path.GetFileNameWithoutExtension(file);
                    if (LuaManager.Assemblys.ContainsKey(file1))
                        LuaManager.Assemblys.Remove(file1);
                }
                foreach (var file in modifyFiles) {
                    foreach (var lua in luas) {
                        if (lua.csSourceFile == null)
                            continue;
                        if (lua.csSourceFile.name == file)
                            lua.Init();
                    }
                }
            }

            if (string.IsNullOrEmpty(path) | string.IsNullOrEmpty(path1))
                return;

            if (changed) {
                changed = false;

                int count = modifyFiles.Count;
                foreach (var file in modifyFiles) {
                    var bytes = File.ReadAllBytes(file);
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var file1 = path1 + "/" + fileName + ".txt";
                    File.WriteAllBytes(file1, bytes);

                    if (LuaManager.Assemblys.ContainsKey(fileName))
                        LuaManager.Assemblys.Remove(fileName);
                }

                Debug.Log("自动更新成功!");
                AssetDatabase.Refresh();

                var luas = FindObjectsOfType<LuaComponent>();
                foreach (var file in modifyFiles) {
                    var file1 = Path.GetFileNameWithoutExtension(file);
                    foreach (var lua in luas) {
                        if (lua.csSourceFile == null)
                            continue;
                        if (lua.csSourceFile.name == file1)
                            lua.Init();
                    }
                }

                modifyFiles.RemoveRange(0, count);
            }

            if (watcher == null & !string.IsNullOrEmpty(path)) {
                Changed(path.Replace("/", "\\") + "\\");
            }

            Repaint();
        }
    }
}