// Editor/MaterialLockToolImportHook.cs
// UnityPackage インポート完了時に自動ロックを適用するフック
// Auto Lock on Package Import が ON の場合のみ動作する

using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class MaterialLockToolImportHook
{
    static MaterialLockToolImportHook()
    {
        AssetDatabase.onImportPackageItemsCompleted += OnPackageImported;
    }

    static void OnPackageImported(string[] importedPaths)
    {
        if (!MaterialLockToolSettings.instance.autoLockOnImport) return;

        // Material アセットのパスのみに絞り込む
        var materialPaths = importedPaths
            .Where(p => AssetDatabase.LoadMainAssetAtPath(p) is Material)
            .ToArray();

        if (materialPaths.Length == 0) return;

        // インポート処理の完全終了を待ってからロックを適用
        EditorApplication.delayCall += () =>
            MaterialLockToolHelper.LockMaterials(materialPaths);
    }
}
