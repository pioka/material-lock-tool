// Editor/MaterialLockToolMenu.cs
// 右クリックメニュー（ロック/解除）と
// Tools > Material Lock Tool > Auto Lock on Package Import のトグルを提供する

using System.Linq;
using UnityEditor;
using UnityEngine;

public static class MaterialLockToolMenu
{
    const string MenuLock     = "🔒 Lock Material Properties";
    const string MenuUnlock   = "🔓 UnLock Material Properties";
    const string MenuAutoLock = "Tools/Material Lock Tool/Auto Lock on Package Import";

    // ────────────────────────────────
    //  起動・ドメインリロード時の初期化
    // ────────────────────────────────
    [InitializeOnLoadMethod]
    static void Init()
    {
        // メニューシステムの準備完了後にチェック状態を反映する（NDMFと同じパターン）
        EditorApplication.delayCall += SyncMenuCheckState;
    }

    // ────────────────────────────────
    //  右クリック: ロック
    // ────────────────────────────────
    [MenuItem(MenuLock, true)]
    static bool LockValidate() => HasTarget() && !AllLocked();

    [MenuItem(MenuLock)]
    static void Lock()
    {
        MaterialLockToolHelper.LockMaterials(
            GetTargets().Select(AssetDatabase.GetAssetPath).ToArray()
        );
    }

    // ────────────────────────────────
    //  右クリック: 解除
    // ────────────────────────────────
    [MenuItem(MenuUnlock, true)]
    static bool UnlockValidate() => HasTarget() && AnyLocked();

    [MenuItem(MenuUnlock)]
    static void Unlock()
    {
        MaterialLockToolHelper.UnlockMaterials(
            GetTargets().Select(AssetDatabase.GetAssetPath).ToArray()
        );
    }

    // ────────────────────────────────
    //  Toolsメニュー: Auto Lock トグル
    // ────────────────────────────────
    [MenuItem(MenuAutoLock)]
    static void ToggleAutoLock()
    {
        MaterialLockToolSettings.instance.autoLockOnImport =
            !MaterialLockToolSettings.instance.autoLockOnImport;
        MaterialLockToolSettings.instance.Save();
        SyncMenuCheckState();
    }

    static void SyncMenuCheckState()
    {
        Menu.SetChecked(MenuAutoLock, MaterialLockToolSettings.instance.autoLockOnImport);
    }

    // ────────────────────────────────
    //  選択アセットの状態チェック
    // ────────────────────────────────
    static Object[] GetTargets()
    {
        var directMaterials = Selection.objects
            .Where(obj => obj is Material);

        var folderMaterials = Selection.objects
            .Where(obj => obj is DefaultAsset && AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(obj)))
            .SelectMany(folder => AssetDatabase.FindAssets("t:Material", new[] { AssetDatabase.GetAssetPath(folder) }))
            .Select(guid => AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(obj => obj != null);

        return directMaterials.Concat(folderMaterials).Distinct().ToArray();
    }

    static bool HasTarget()  => GetTargets().Length > 0;
    static bool AllLocked()  => GetTargets().All(MaterialLockToolHelper.IsLocked);
    static bool AnyLocked()  => GetTargets().Any(MaterialLockToolHelper.IsLocked);
}
