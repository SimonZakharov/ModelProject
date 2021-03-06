# ModelProject
Тестовое задание для Forward Development. Язык - C#, приложение собрано на .NET Core 2.2.

# Структура проекта
Engine - абстрактный класс, представляющий базовый функционал двигателя (температура, скорость, ускорение, момент инерции).
ModelEngine - класс, расширяющий функционал Engine для решения поставленной задачи моделирования. Наследуется от Engine.
Program - класс содержит точку входа приложения.

# Запуск и тестирование приложения
Приложение работает с аргументами командной строки и ориентировано на файловый ввод-вывод.
В качестве первого аргумента указывается источник входных данных - файл формата txt, данные в котором должны быть записаны в следующем порядке, без каких-либо комментариев:
 - момент инерции I
 - температура перегрева
 - коэффициент Hm
 - коэффициент Hv
 - коэффициент зависимости охлаждения С
 - количество N отрезков в кусочно-линейной зависимости M от V
 - далее N строк по два вещественных числа, разделенных пробелом: M и V соответственно

В качестве второго аргумента указывается имя файла, в который будут записаны пошаговые результаты моделирования (файл не обязательно должен существовать в файловой системе; 
существующий файл будет перезаписан). Второй аргумент не является обязательным; если не указать его, вывод будет произведен на экран.

Если при запуске приложения не указывается ни одного аргумента командной строки, ввод данных производится с клавиатуры, а вывод - на экран.

В данной реализации ввод температуры окружающей среды всегда происходит с клавиатуры.
