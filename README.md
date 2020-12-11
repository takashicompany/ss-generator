# SS Geneator

# できること
- Unity上でスクリーンショットを一定間隔で撮影する
- 撮影したスクリーンショットをストア用にリサイズする
  - iPhone 6.5inch (1242 x 2688)
  - iPhone 5.5inch (1242 x 2208)
  - iPad (2048 x 2732)

# できないこと
- 横長画面のスクリーンショットの撮影
  - そのうち対応する予定
- 画面端にUIが吸着している場合、5.5inchや6.5inchのスクリーンショットで表示されない
  - 3:4で撮影して、端を切り落とす実装なので

# 使い方

## スクリーンショットを撮影する

### 1. 画面のサイズを3:4にする

GameViewの画面をベースにスクリーンショットを撮影するので、GameViewを可能な限り大きくすると良いです。
![image](https://user-images.githubusercontent.com/4215759/101769094-cf25f000-3b29-11eb-880e-2ac60647fe59.png)

### 2. Unityの再生ボタンを押す
![image](https://user-images.githubusercontent.com/4215759/101769346-2c21a600-3b2a-11eb-8803-0af7519c1e58.png)

### 3. メニューバーの「スクショ自動撮影 > 撮影を開始」を実行する
![image](https://user-images.githubusercontent.com/4215759/101769376-380d6800-3b2a-11eb-927b-3dd14764d5e6.png)

1秒毎にスクリーンショットが撮影される。

![image](https://user-images.githubusercontent.com/4215759/101769460-570bfa00-3b2a-11eb-89e4-10af768c2d86.png)

Unityの再生を終了すると自動で撮影が停止される。

### 4. Unityプロジェクト内の「ss」フォルダにスクリーンショットが保存される
![image](https://user-images.githubusercontent.com/4215759/101769558-7dca3080-3b2a-11eb-81e9-47246ba28d47.png)

## リサイズ

### 1. リサイズしたいスクリーンショットを一つのフォルダにまとめる
![image](https://user-images.githubusercontent.com/4215759/101769664-a3573a00-3b2a-11eb-9611-ea0eb02b9a55.png)

枚数が多いとリサイズに多大な時間がかかるので5枚程度を目安にすると良い。

![image](https://user-images.githubusercontent.com/4215759/101769768-d0a3e800-3b2a-11eb-83de-2863e50fa135.png)


### 2. メニューバーの「スクショ自動撮影 > フォルダ内の画像をストア用にリサイズ」を実行する
![image](https://user-images.githubusercontent.com/4215759/101769823-e44f4e80-3b2a-11eb-88a0-f5d641e927d9.png)

### 3. 1.で選んだスクリーンショットの入ったフォルダを選択する
![image](https://user-images.githubusercontent.com/4215759/101769870-faf5a580-3b2a-11eb-8c2a-cc51ac3e1e2b.png)

### 4. リサイズが自動で実行される
![image](https://user-images.githubusercontent.com/4215759/101769918-0cd74880-3b2b-11eb-9f0e-964f7707be0b.png)

### 5. サイズ別にスクリーンショットが書き出される
![image](https://user-images.githubusercontent.com/4215759/101770007-2aa4ad80-3b2b-11eb-8ba5-8c5eee720139.png)


