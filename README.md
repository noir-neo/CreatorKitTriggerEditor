# Creator Kit Trigger Editor

[Cluster Creator Kit](https://github.com/ClusterVR/ClusterCreatorKit) の Trigger -> Gimmick の流れをちょっとだけ追いやすくするやつ。

## 要件

- Unity 2019.4
- Cluster Creator Kit v1.14.1 で動作確認済み

## インストール

### from disk

1. 最新の [release](https://github.com/noir-neo/CreatorKitTriggerEditor/releases) からダウンロード、展開します
1. Unity メニュー Window > Package Manager から "Packages" ウィンドウを開きます
1. ウィンドウ内左上の "+" ボタンから "Add package from disk" を選択、展開したフォルダ内の `package.json` を開きます

### [scoped registries](https://docs.unity3d.com/Manual/upm-scoped.html)

1. Packages/manifest.json を開き、以下のように編集します

```Packages/manifest.json
{
  "scopedRegistries": [
    {
      "name": "noir_neo",
      "url": "https://registry.npmjs.com",
      "scopes": [ "com.neoneobeam" ]
    }
  ],
  "dependencies": {
    "com.neoneobeam.cluster-creator-kit.trigger-editor": "0.1.4",
    ...
```

## 使い方

### StateKeyListWindow

<img width="521" src="https://user-images.githubusercontent.com/3272594/102250045-67ffa580-3f46-11eb-83d7-ecec4bd8607c.png">

1. Unity メニュー CreatorKitTriggerEditor > Open Window から "StateKeyListWindow" ウィンドウ を開きます
1. "Refresh List" ボタンを押すと、現在開いている scene で使用しているトリガー・ギミックの key の一覧が表示されます
1. 行を選択すると、その key を使用している component が選択されます

## ライセンス

MIT License
