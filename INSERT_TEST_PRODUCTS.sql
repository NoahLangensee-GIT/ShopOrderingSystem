-- Testprodukte in die Product-Tabelle einfügen
-- ProductCategory: Elektronik=0, Kleidung=1, Lebensmittel=2

INSERT INTO Product (Name, Price, Category) VALUES
-- Elektronik
('Laptop Dell XPS', 1299.99, 0),
('Wireless Maus Logitech', 45.99, 0),
('USB-C Kabel (2m)', 12.99, 0),
('Monitor LG 27 Zoll', 349.99, 0),
('Tastatur Mechanisch RGB', 129.99, 0),

-- Kleidung
('T-Shirt weiß', 19.99, 1),
('Jeans blau', 59.99, 1),
('Hoodie grau', 49.99, 1),
('Socken (6er Pack)', 9.99, 1),
('Running-Schuhe Nike', 119.99, 1),

-- Lebensmittel
('Kaffee Arabica 500g', 8.99, 2),
('Brot Vollkorn', 3.49, 2),
('Käse Emmental 200g', 5.99, 2),
('Milch 1L', 1.49, 2),
('Öl Olivenöl extra vergine', 12.99, 2);

