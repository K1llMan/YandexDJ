# YandexDJ
Веб-сервер приложения для стримов.

## Функционал
1. Интеграция с Яндекс.Музыкой и локальными плейлистами.
2. Бот для твича.
3. Оверлей для стрима с настройкой отображаемых виджетов.
4. Смена схем оверлея на лету.
5. Плеер для плейлистов из интегрированных сервисов с выравниваем громкости по усилению.

## Запуск в отладке
Необходимо предварительно добавить следующие проекты:
- [Yandex.Music API (Unofficial) for .Net Core](https://github.com/K1llMan/Yandex.Music.Api) в ../Yandex.Music.Api
- [YandexDJ React App](https://github.com/K1llMan/YandexDJReactApp) в ./ClientApp
Затем собрать из ClientApp веб-приложение. После этого в отладке будет доступен интерфейс веб-приложения по локальному адресу, оверлей - по {локальный адрес}/stream.
При сборке в выходную директорию будут добавлены тестовые данные для бота, оверлея и локальных плейлистов.
Настройки самого приложения располагаются в settings.json. Пример заполнения находится в файле settings_example.json.
