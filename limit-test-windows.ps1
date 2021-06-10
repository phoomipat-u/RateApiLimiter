For ($i=1; $i -le 25; $i++) {
    # Write-Output $i
    $result = try {Invoke-WebRequest -Uri http://localhost:5001/hotel/city?city=Bangkok -Method Get | Select-Object -Expand StatusCode} catch { $_.Exception.Response.StatusCode }
    Write-Output "Call $i result is $result"
}