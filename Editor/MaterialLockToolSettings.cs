// Editor/MaterialLockToolSettings.cs
// 設定の永続化
// UserSettings/MaterialLockTool/settings.asset に保存される（個人設定・Git管理対象外）

using UnityEditor;
using UnityEngine;

[FilePath("MaterialLockTool/settings.asset", FilePathAttribute.Location.PreferencesFolder)]
internal class MaterialLockToolSettings : ScriptableSingleton<MaterialLockToolSettings>
{
    [SerializeField] public bool autoLockOnImport = false;

    public void Save() => Save(true);
}
