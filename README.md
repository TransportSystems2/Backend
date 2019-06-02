[![Documentation Status](https://readthedocs.org/projects/gosevakuator/badge/?version=latest)](https://gosevakuator.readthedocs.io/en/latest/?badge=latest)[![CodeFactor](https://www.codefactor.io/repository/github/transportsystems2/backend/badge)](https://www.codefactor.io/repository/github/transportsystems2/backend)

master [![Build Status](https://travis-ci.com/TransportSystems2/Backend.svg?branch=master)](https://travis-ci.com/TransportSystems2/Backend)

develop [![Build Status](https://travis-ci.com/TransportSystems2/Backend.svg?branch=develop)](https://travis-ci.com/TransportSystems2/Backend)


чтобы докер правильно работал на винде, надо создать локальные тома.
Эти тома будут использоваться для того, чтобы хранить данные баз данных локально на компьютере, а не в образе.
Без этих томов никак, потому что файловая система винды(ntfs) плохо дружит с файловой системой линуксов и напрямую сопоставить локальную виндовую папку с папкой внутри линуксового контейнера не получится
Виртуальные тома в винде решают эту проблему.

Чтобы создать виртуальный том, в котором будут храниться данные для ядра, надо выполнить команду в cmd, можно не в корне проекта:
docker volume create --name postgres-core-data -d local

Чтобы создать виртуальный том, в котором будут храниться данные для identity, надо выполнить команду в cmd, можно не в корне проекта:
docker volume create --name postgres-identity-data -d local

чтобы посмотреть список созданных томов, надо выполнить команду в cmd, можно не в корне проекта:
docker volume ls.

Потом все как обычно.
docker-compose down
docker-compose up
docker-compose up --build
