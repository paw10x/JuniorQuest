# Wizja projektu

## Problem
Szukanie pierwszej pracy wiąże się z wieloma repetytywnymi zadaniami, takimi jak przeglądanie setek ogłoszeń, 
sprawdzanie maili czy personalizacja CV. Nie budują one wartości w procesie rekrutacyjnym, a jedynie generują frustrację 
i stratę czasu. Dotyka to między innymi studentów informatyki wchodzących na rynek pracy, którzy nie mogą skupić się na 
ważniejszych taskach.

## Użytkownicy
Studenci informatyki szukający pierwszej pracy w IT

## Scenariusze
- Przegląd ofert: Student otwiera aplikację raz dziennie, aby zobaczyć przefiltrowaną listę nowych ogłoszeń dopasowanych do jego zainteresowań
- Automatyczne dostosowanie CV: Użytkownik widzi interesującą ofertę i jednym kliknięciem generuje spersonalizowaną wersję swojego 
CV pod kątem wymaganych w ogłoszeniu słów kluczowych - którą może wykorzystać do sprawniejszej aplikacji
- Zarządzanie aplikacjami: Śledzenie statusów wysłanych zgłoszeń (wysłane, odrzucone, zaproszenie na rozmowę) w jednym miejscu 
bez konieczności przeszukiwania skrzynki mailowej

## Rola agenta AI
- Autonomiczne przeszukiwanie i parsowanie: Agent przeszukuje wiele platform z ogłoszeniami (np. JustJoinIT, LinkedIn, No Fluff Jobs), pobiera opisy, kategoryzuje je i wyciąga kluczowe wymagania
- Inteligentny matching i personalizacja: Agent analizuje profil oraz projekty użytkownika z GitHub, a następnie dopasowuje treść CV oraz listu motywacyjnego pod konkretną ofertę pracy
- Orkiestracja zadań: Automatyzuje powtarzalny cykl życia rekrutacji, pozwalając użytkownikowi skupić się wyłącznie na przygotowaniu do rozmów technicznych

## Zakres
Co planujemy zrealizować:
- Zbieranie i agregacja ofert z popularnych portali
- System profilu użytkownika (oparty o różne informacje przez niego podane)
- Generator dopasowanych CV / podsumowań przy użyciu LLM
- Prosty pulpit (dashboard) do śledzenia statusów aplikacji i rekomendacji kolejnych akcji

Czego projekt NIE obejmuje (w pierwszej fazie):
- Automatycznego wysyłania formularzy aplikacyjnych na zewnętrznych stronach
- Wbudowanego komunikatora do rozmów z rekruterami
- Płatnych subskrypcji i zaawansowanych systemów rekomendacji opartych o głęboką analizę psychometryczną