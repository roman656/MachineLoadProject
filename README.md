# MachineLoadProject

## Описание
Имитационная модель фрагмента производственной системы в виде автоматического участка построена на **дискретно-событийном принципе**.

Участок состоит из *4-х станков с ЧПУ* заданного вида, обслуживаемых *промышленным роботом*.

Каждый станок выполняет свой технологический процесс, причем эти процессы могут быть различными или одинаковыми для всех или части станков.
Содержание каждой из технологических операций в данном случае не имеет значения.
При завершении своего технологического процесса станок дает заявку в общую управляющую ЭВМ, которая ставит его в очередь на обслуживание.
Станок начинает простаивать в ожидании обслуживания (режим - «простой»).

Далее, в порядке очередности, промышленный робот забирает готовую деталь и устанавливает очередную заготовку на станок и он переходит в рабочий режим (режим - «работа»).
После чего робот движется к тактовому столу, где укладывает готовую деталь и берет следующую заготовку.
Если очередь из заявок отсутствует, робот переходит в режим «простой», если заявка на обслуживание присутствует, робот переходит в режим «работа» и начинает движение к станку, подавшему заявку на обслуживание. Далее процесс повторяется.

## Входные данные
- Время выполнения технологической операции на каждом станке
- Скорость движения робота и расстояние до каждого из станков
- Время непосредственного обслуживания каждого из станков
- Время взаимодействия робота с тактовым столом

## Дополнительные данные
- Смена длится 8 часов
- На момент начала смены все станки уже готовы к работе

## Выходные данные
- Время простоя и время работы каждой из единиц технологического оборудования, включая робота
- Коэффициент загрузки каждой из единиц технологического оборудования (время работы / время работы и простоя)

## Результаты
Вывод программы (сокращенный):
```
Время смены: 28800
Коэффициент загрузки робота:      0,809  Работал / Простаивал: 23301 / 5499  Количество обслуживаний станков:  267
Коэффициент загрузки 1-го станка: 0,710  Работал / Простаивал: 20442 / 8358  Количество произведенных деталей: 113
Коэффициент загрузки 2-го станка: 0,740  Работал / Простаивал: 21301 / 7499  Количество произведенных деталей: 71
Коэффициент загрузки 3-го станка: 0,825  Работал / Простаивал: 23746 / 5054  Количество произведенных деталей: 56
Коэффициент загрузки 4-го станка: 0,834  Работал / Простаивал: 24032 / 4768  Количество произведенных деталей: 28
```
Вывод программы (полный):
```
Количество станков: 4
Время смены: 28800

Робот:
  Время взаимодействия с тактовым столом: 10
  Скорость движения: 0,4
  Работал: 23301
  Простаивал: 5499
  Коэффициент загрузки: 0,8090625
  Количество обслуживаний станков: 267

Станок №1:
  Расстояние до станка: 5
  Время выполнения технологической операции: 180
  Время обслуживания: 30
  Работал: 20442
  Простаивал: 8358
  Коэффициент загрузки: 0,70979166
  Количество произведенных деталей: 113

Станок №2:
  Расстояние до станка: 7
  Время выполнения технологической операции: 300
  Время обслуживания: 60
  Работал: 21301
  Простаивал: 7499
  Коэффициент загрузки: 0,73961806
  Количество произведенных деталей: 71

Станок №3:
  Расстояние до станка: 9
  Время выполнения технологической операции: 420
  Время обслуживания: 30
  Работал: 23746
  Простаивал: 5054
  Коэффициент загрузки: 0,8245139
  Количество произведенных деталей: 56

Станок №4:
  Расстояние до станка: 11
  Время выполнения технологической операции: 840
  Время обслуживания: 60
  Работал: 24032
  Простаивал: 4768
  Коэффициент загрузки: 0,83444446
  Количество произведенных деталей: 28
```
