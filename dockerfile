# 公式のGolangイメージをベースとする。タグは好みのバージョンに合わせて変更可能。
FROM golang:1.22-alpine

# アプリケーションのソースコードを含むディレクトリを作成
WORKDIR /app

# ホストマシン上のソースコードをコンテナ内の作業ディレクトリにコピー
COPY . .

# 依存関係のインストール
RUN go mod download

# アプリケーションのビルド。出力ファイル名はapp。
RUN go build -o /docker-golang-app

# コンテナ起動時にアプリケーションを実行
CMD ["/docker-golang-app"]
