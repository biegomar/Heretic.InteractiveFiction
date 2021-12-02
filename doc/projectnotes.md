# Hints für ein Benutzerhandbuch

* es ist wichtig, sich die Texte der Objekte, die man sich durchliest genauer anzuschauen. Es gibt oft einen ersten Eindruck, der sich bei wiederholtem Lesen nicht mehr offenbart.

# Versionsnummer

* Stelle 1: Hauptversion. Bis zur Fertigstellung 0
* Stelle 2: Nebenversion. Gibt die Nummer des gerade in Entwicklung befindlichen Haupträtsel an. Rätsel 1: Pendel lösen und Kette bekommen.
* Stelle 3: Patch. Gibt die Anzahl der Einzelaufgaben an, die mit Punkten bewertet werden.

# Technische Details

* der VerbHandler führt für die meisten Verben eine Basisfunktionalität aus. Weitere Aktionen darüber hinaus sind mittels der EventHandler zu realisieren.
* Die Rückgabewerte der verbs im VerbHandler signalisieren lediglich, dass es sich um das korrekte Verbs gehandelt hat und irgendetwas gemacht wurde. Er hat ansonsten keine weitere Bedeutung.
* Events haben nie einen Rückgabewert.
* Es gibt Rätsel, die man durch unterschiedliche Events lösen kann. Der erste gewählte Weg soll dann die Ausführung der weiteren Events nicht mehr zulassen. Jeder mögliche Event erhält eine Punktzahl, aber generell soll die Aktion nur einmal in den möglichen Gesamtscore einfliessen. Um das zu erreichen gibt es das Flag "CountForOverallScore", das für alle Events aus dem Block bis auf eines, auf false gesetzt werden soll.
* Will man unwichtige Objekte untersuchen, ohne sie aber in der Liste der sichtbaren Objekte aufzuführen (z.B. Sushi in Fish Bowl), kann man einfach einen Eintrag in dem Dictionary "Surroundings" hinterlegen. Leider sind aber weiterhin die gesamten Keys in den entsprechenden Ressourcen-Files zu hinterlegen (also: Keys, Description, Items)
* Die Klasse Universe hat nun die Möglichkeit gelöste Rätsel zu verwalten.
