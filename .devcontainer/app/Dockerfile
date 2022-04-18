FROM mcr.microsoft.com/dotnet/sdk:6.0-focal

# タイムゾーンや言語を日本にする
ENV TZ=Asia/Tokyo \
    LANG=ja_JP.UTF-8 \
    LANGUAGE=ja_JP:ja \
    LC_ALL=ja_JP.UTF-8

RUN echo "${TZ}" > /etc/timezone && dpkg-reconfigure -f noninteractive tzdata

# OS の言語を日本語にすることでロケールを使う関数を使えるようにする
RUN apt-get update && apt-get upgrade -y \
    && apt-get install -y locales \
    && locale-gen ja_JP.UTF-8 \
    && localedef -f UTF-8 -i ja_JP ja_JP.UTF-8

# SSH クライアントが入っていないので
RUN apt-get update \
    && apt-get install -y openssh-client

# apt でのパッケージを削除
RUN apt-get clean \
    && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /workspace
WORKDIR /workspace
