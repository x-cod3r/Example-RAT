<?php

// Establish a connection to the MySQL database
$db = new mysqli('localhost', 'root', '', 'controlserver');

// Check for connection errors
if (mysqli_connect_errno()) {
    // If there's an error in the database connection, exit the script
    exit;
}

// Retrieve POST data sent from an external source (e.g., a client-side application or system)
$name = $_POST["name"];     // Name of the client
$ip = $_POST["ip"];         // IP address of the client
$id = $_POST["id"];         // Unique ID of the client
$os = $_POST["os"];         // Operating system information of the client
$country = $_POST["country"]; // Country information of the client

// Prepare an SQL INSERT statement to insert data into the 'clients' table
$query = "INSERT INTO clients(name, ip, id, os, country) VALUES(?,?,?,?,?)";
$stmt = $db->prepare($query);

// Bind the received POST data to the SQL query using 'bind_param' to prevent SQL injection
$stmt->bind_param('sssss', $name, $ip, $id, $os, $country);

// Execute the SQL statement to insert the provided information into the database
$stmt->execute();

// Close the database connection after the insertion operation
$db->close();
?>
