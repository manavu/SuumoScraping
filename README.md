# SuumoScraping

## ���\�z

Visual Studio 2019 �ȍ~���C���X�g�[�����܂��B

.net core sdk 3.1 ���C���X�g�[�����܂��B

DB��MySql5.7 �ȍ~���C���X�g�[�����ĕK�v�Œ���̊����\�z���Ƃ��܂��B

Visual Studio ���N�����A�\�����[�V�������r���h���܂��B

�p�b�P�[�W�}�l�[�W���R���\�[�����J���܂��B
���L�̃R�}���h���C����ConnectionString �̈�����K�؂Ȓl�ɕύX���ăp�b�P�[�W�}�l�[�W���R���\�[��������s���܂��B

```
update-database -verbose -ConnectionProviderName "MySql.Data.MySqlClient" -ConnectionString "server=localhost;database=ScrapingDb;port=3306;characterset=utf8;uid=****;pwd=****;"
```

�A�v���P�[�V�����̎��s���ɂ̓V�[�N���b�g���g���Đڑ��������n���Ă���̂ŁA���L�̃R�}���h�ŃV�[�N���b�g��ǉ����܂��B

```
dotnet user-secrets init --id "bde44560-6d21-40eb-bd09-82c35fa5c7cf"
dotnet user-secrets set "ConnectionStrings:ScrapingDb" "server=localhost;database=ScrapingDb;port=3306;uid=****;pwd=****;characterset=utf8;"
```
