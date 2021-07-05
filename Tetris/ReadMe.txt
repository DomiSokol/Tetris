Tetris




Im Package.appxmanifest wurde im Code in Zeile 50 bei <uap:SplashScreen> der Parameter a:Optional="true" zusätzlich hinzugefügt. Dies bewirkt, dass der Splash-Screen nicht auftaucht, wenn die Ladezeit gering ist.

Ebenfalls wurde im appxmanifest einzelne Visuelle Assets hinzugefügt (Kleine Kachel, Mittelgroße Kachel, Breite Kachel, Große Kachel und App-Symbol)
Beim Begrüßungsbildschirm wurde hier noch die Hintergrundfarbe auf Schwarz gesetzt und ebenfalls ein Asset hinzugefügt

Die einzelnen Assets wurden alle selbstständig gefertigt, abgesehen von den Music-Files und den verwendeten Fonts.





Die Steine/Tetrons:

Durch die Klassen "Tetron" und "Block" werden die einzelnen Spielsteine (genannt Tetron) erstellt. Um das Bewegen der Tetron auf dem Spielfeld zu gewährleisten, wurden die einzelnen Blöcke in der Klasse Tetron zu einem gemeinsamen Canvas hinzugefügt. Diese Lösung bietet allgemein viele Erleichterungen, vor allem während des Drehens der Tetron.

- Die einzelnen Tetron-Objekte bestehen aus Block-Objekten, welche die einzelnen Rechtecke darstellen
- Block-Objekte werden in einem Array im Tetron-Objekt gespeichert. Dies wird für die logische Sicht des Spiels im Hintergrund genutzt. Beim Typ 1 reicht ein 2x2 Array aus. Bei den Typen 2-6 jeweils ein 3x3 Array und beim letzten Typen wird ein 4x4 Array benötigt, damit die Rotationsfunktion funktioniert.
- Die Rectangles, welche in der Klasse "Block" erstellt werden, werden hier in ein gemeinsamen Canvas hinzugefügt, um den gesamten Tetron darstellen zu können




Das Spielfeld:

Das Spielfeld wird logisch, als zweidimensionales Array vom Typ Block, gesehen. Einerseits gibt es das offizielle Spielfeld, was den aktuellen, visuellen Stand widerspiegelt. Andererseits gibt es das Spielfeld zum Rotieren der Steine
-	Das Spielfeld zum Rotieren dient dazu, damit hinzugefügte Steine (ob bei GameOver, Rotating oder normales Bewegen) die richtige Position im richtigen Spielfeld bekommen




Tauschen der Tetrons:

Das Tauschen der Steine wird nur einmal pro neuem Tetron erlaubt. Ein Zurücktauschen des aktuell neu getauschten Tetrons ist nicht erlaubt.




Bepunktung des Spiels:

"Softdrop" bedeutet, der Stein wird von dem Spieler durch das drücken der Taste "S" schneller als normal nach unten bewegt. Jede einzelne Linie gibt hier einen Punkt.
"Combos" werden berechnet mit dem festen Wert 50 multipliziert mit der Anzahl der Combos und dem aktuellen Level.




Level-Up:

Je 10 zerstörten Reihen gibt es ein Level-Up. Bei 150 Reihen gibt es ein Ende, bei Level 15.