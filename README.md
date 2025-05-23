# Algorytm węgierski – maksymalne skojarzenie w grafie dwudzielnym

Program realizuje algorytm węgierski służący do znajdowania skojarzenia o maksymalnej sumie wag w ważonym grafie dwudzielnym. Dane wejściowe i wyjściowe są reprezentowane w formacie tekstowym zgodnym z wymaganiami zadania.

---

## Struktura

- `Program.cs` – główny punkt wejścia programu.
- `InputGenerator.cs` – narzędzie do generowania losowych plików wejściowych.
- `HungarianAlgorithm/` – folder zawierający klasy:
  - `Vertex.cs` – reprezentacja wierzchołka.
  - `Edge.cs` – reprezentacja krawędzi z wagą.
  - `HungarianAlgorithm.cs` – implementacja algorytmu.

---

## Format pliku wejściowego

Plik tekstowy `input.txt` zawiera wagową macierz sąsiedztwa grafu dwudzielnego \( G = (L \cup R, E) \) rozmiaru \( n \times n \), gdzie:

- wiersze odpowiadają wierzchołkom z części lewej \( L \),
- kolumny – z części prawej \( R \),
- wartość `n` (mała litera) oznacza brak krawędzi (czyli nieskończoną wagę),
- wartości oddzielone są spacją.

### Przykład:

```
n 10
2 5
```

Oznacza:
- \( L = \{0, 1\} \), \( R = \{2, 3\} \)
- Istnieją krawędzie:
  - \( (0, 3) \) o wadze 10
  - \( (1, 2) \) o wadze 2
  - \( (1, 3) \) o wadze 5

---

## Format pliku wyjściowego

Plik `output.txt` zawiera:

1. W pierwszej linii: łączną wagę skojarzenia,
2. W kolejnych wierszach: pary wierzchołków `L R` tworzące skojarzenie.

### Przykład:

```
9
0 4
1 3
2 5
```

---

## Uruchamianie

Przed uruchomieniem programu należy przygotować plik wejściowy `input.txt`. Alternatywnie, możliwe jest automatyczne wygenerowanie przykładowego pliku za pomocą poniższej komendy (n oznacza liczność jednego zbioru):

```bash
dotnet run --random <n>
```

Aby przeprowadzić testy działania programu, należy dodać pakiet:

```bash
dotnet add package ScottPlot
```

Następnie należy użyć komendy:

```bash
dotnet run --test
```
Opcjonalnie można przeprowadzić test na numrowanych plikach input_{nr}.txt stosując taką komendę, należy wcześniej przygotowac taki plik, przygotowalismy kilka o nr 1,2,3,4,5,100 :

```bash
dotnet run --test <n>
```
