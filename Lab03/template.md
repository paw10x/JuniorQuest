# Raport — Laboratorium 03

**Zespół:** JuniorQuest
**Data:** 26.10.2026

## Środowisko

| Parametr | Wartość                           |
|----------|-----------------------------------|
| Wersja Ollama | 0.40.8                            |
| Sprzęt (CPU/GPU, RAM) | Intel Core Ultra 5 125H, 16GB RAM |
| Liczba dostępnych notatek | 170                               |

## EXP-00 — Sposób oceny wyników

Jak oceniamy poprawność odpowiedzi modelu? Jakie metryki przyjmujemy i dlaczego?

| Metryka | Definicja |
|---------|-----------|
| Exact match | Odsetek notatek, dla których oba pola są poprawne |

## EXP-01 — Porównanie modeli

| Model                             | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-----------------------------------|----------------|-------------------|-------------|------------------|
| qwen3:4b                          | 100%           | 100%              | 100%        | 62185            |
| maternion/minicpm-v4.7-heretic:1b | 40%            | 40%               | 30%         | 3600             |

**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.

**Wnioski:** Model Qwen3:4b wykazał bezbłędną skuteczność (100% Exact match). Z kolei minicpm-v4.7-heretic:1b działał ponad 17 razy szybciej (3,6 s vs 62,2 s), co wynika z jego mniejszej skali, jednak osiągnął zdecydowanie niższą dokładność (30% Exact match). Zysk na wydajności mniejszego modelu odbywa się w tym przypadku kosztem drastycznego spadku jakości, co czyni go nieprzydatnym bez dodatkowej optymalizacji.

## EXP-02 — Prompting

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
|---------|----------------|-------------------|-------------|
| zero-shot | 40%            | 40%               | 30%         |
| one-shot | 80%            | 40%               | 30%         |
| few-shot | 90%            | 60%               | 50%         |

**Wnioski:** Zastosowanie technik in-context learning znacząco poprawia skuteczność modelu, zwiększając wskaźnik Exact match z 30% (zero-shot) do 50% (few-shot). Największą poprawę zaobserwowano w precyzji pola floor (wzrost z 40% do 90%), podczas gdy rozpoznawanie pola elevator okazało się bardziej wymagające (max. 60%). Wyniki jednoznacznie wskazują, że dostarczenie przykładowych Notatek w prompcie (few-shot) skutecznie ukierunkowuje model na poprawne ekstrakcje.

## EXP-03 — Temperature

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| 0.0 | 10%         | 10/10               |
| 0.5 | 28%         | 0/10                |
| 1.0 | 18%         | 0/10                |

**Wnioski:** Parametr temperatury wynoszący 0.5 okazał się najbardziej optymalny, zapewniając najwyższą dokładność (Exact match 28%) przy całkowitej eliminacji powtarzalnych, zapętlonych odpowiedzi (0/10). Zerowa temperatura (T=0.0) zagwarantowała co prawda 100% powtarzalności notatek, jednak drastycznie obniżyła precyzję modelu (tylko 10%). Wyższe wartości temperatury (T=1.0) wprowadziły zbyt dużą losowość, co przełożyło się na spadek dokładności do 18%.

## EXP-04 — Structured output — JSON i walidacja

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | 8/10          | 30%         |
| json-schema | 10/10         | 30%         |

**Wnioski:** Wymuszenie formatu za pomocą schematu JSON pozwoliło na całkowite wyeliminowanie błędów strukturalnych, osiągając 100% poprawności struktury (10/10) w porównaniu do 8/10 przy tradycyjnym prompcie. Jednocześnie zastosowanie walidacji nie wpłynęło na merytoryczną dokładność ekstrakcji danych, utrzymując wskaźnik Exact match na niezmienionym poziomie 30%. Oznacza to, że JSON-schema gwarantuje stuprocentową poprawność techniczną odpowiedzi bez zmiany zdolności analitycznych modelu.

## Najtrudniejsze notatki

Model miał największe problemy z wykrywaniem wind, np. "Winda nie działa" model często zapisywał jako elevator=true.
Model "drugie lub trzecie piętro" często zapisywał jako floor=2 lub floor=3 zamiast floor=null, nie rozpoznając, że nie jest
wiadomo, o które piętro chodzi. "Mieszkanie 15, domofon 15" model interpretował jako floor=15. Z niewiadomych przyczyn "winda po prawej"
często było rozpoznawane jako winda=null.

## Rekomendacja

Który model i prompt wybieramy do projektu — i jakie mamy dowody, że to dobry wybór?

Wybieramy testowany model Qwen, który ma zdecydowanie wyższą celność w rozpoznawaniu szukanych wartości. W projecie będziemy
używali wariantu few-shot, który okazał się być najbardziej skuteczny. Zastosujemy również nieskrajną temperaturę,
ponieważ skrajne wykazały się niższą skutecznością.
