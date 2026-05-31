// Editor/MaterialLockToolHelper.cs
// ロック/解除の実処理
// 右クリックメニューとインポートフックの両方から呼ばれる共通処理をまとめる
// Material Variant は型として Material と同じため、is Material で両方カバーできる

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

internal static class MaterialLockToolHelper
{
    /// 指定パスのマテリアルに NotEditable フラグを立てる
    public static void LockMaterials(string[] paths)
    {
        int count = 0;
        foreach (var path in paths)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(path);
            if (obj is not Material || IsLocked(obj)) continue;

            obj.hideFlags |= HideFlags.NotEditable;
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssetIfDirty(obj);
            count++;
        }
        if (count > 0) RefreshInspectors();
        Debug.Log($"[MaterialLockTool] {count} 件をロックしました。");
    }

    /// 指定パスのマテリアルの NotEditable フラグを下ろす
    public static void UnlockMaterials(string[] paths)
    {
        int count = 0;
        foreach (var path in paths)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(path);
            if (obj is not Material || !IsLocked(obj)) continue;

            obj.hideFlags &= ~HideFlags.NotEditable;
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssetIfDirty(obj);
            count++;
        }
        if (count > 0) RefreshInspectors();
        Debug.Log($"[MaterialLockTool] {count} 件をロックを解除しました。");
    }

    /// NotEditable フラグが立っているか
    public static bool IsLocked(Object obj) =>
        (obj.hideFlags & HideFlags.NotEditable) != 0;

    /// インスペクタの編集可否（NotEditable によるグレーアウト）を即時反映させる。
    /// 編集可否は Editor 生成時の hideFlags を基に決まるため、フラグを書き換えても
    /// 表示中のインスペクタには反映されない。Editor 群を再構築してから再描画する。
    static void RefreshInspectors()
    {
        ActiveEditorTracker.sharedTracker.ForceRebuild();
        InternalEditorUtility.RepaintAllViews();
    }
}
