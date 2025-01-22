param (
	[Parameter(Mandatory = $true)]
	[string]$SpecFile
)

function Replace-RefsRecursively {
	param (
		[PSCustomObject]$Object,
		[hashtable]$ModelMap
	)
	foreach ($property in $Object.PSObject.Properties) {
		if ($property.Value -is [PSCustomObject]) {
			# Recursively handle nested objects
			Replace-RefsRecursively -Object $property.Value -ModelMap $ModelMap
		} elseif ($property.Value -is [System.Collections.IEnumerable] -and -not ($property.Value -is [string])) {
			# Recursively handle arrays
			foreach ($item in $property.Value) {
				if ($item -is [PSCustomObject]) {
					Replace-RefsRecursively -Object $item -ModelMap $ModelMap
				}
			}
		} elseif ($property.Name -eq '$ref') {
			# Update $ref if it matches a model in the map
			$refValue = $property.Value -replace '.*/'
			if ($ModelMap.ContainsKey($refValue)) {
				$property.Value = $property.Value -replace $refValue, $ModelMap[$refValue]
			}
		}
	}
}

# Recursive function to fix invalid $ref entries
function Fix-Refs {
	param ($node, $parent)

	foreach ($key in $node.PSObject.Properties.Name) {
		$value = $node.$key

		if ($key -eq '$ref' -and $value -match '^#/components/schemas/#/.*') {
			# Split the ref path by '/' and remove everything before the last 'properties'
			$refParts = $value -split '/'
			$lastPropertiesIndex = [Array]::LastIndexOf($refParts, 'properties')

			if ($lastPropertiesIndex -gt 0) {
				# Extract the relevant part after the last 'properties'
				$relevantParts = $refParts[$lastPropertiesIndex..($refParts.Length - 1)]
				$parentPath = $value -replace '(/properties/.*)', ''  # Get the base path without the invalid part
				$newRef = "$parentPath/properties/$($relevantParts -join '/')"
				$node.$key = $newRef
			}
		} elseif ($value -is [PSCustomObject]) {
			Fix-Refs -node $value -parent $node
		} elseif ($value -is [System.Collections.IEnumerable]) {
			foreach ($item in $value) {
				if ($item -is [PSCustomObject]) {
					Fix-Refs -node $item -parent $node
				}
			}
		}
	}
}

try {
	# Load the OpenAPI spec
	$spec = Get-Content -Raw -Path $SpecFile | ConvertFrom-Json

	# Check if components.schemas exists
	if (-not $spec.components -or -not $spec.components.schemas) {
		throw "The input file does not contain a valid 'components.schemas' section."
	}
	$schemas = $spec.components.schemas

	# Validate that schemas is a PSCustomObject
	if (-not ($schemas -is [PSCustomObject])) {
		throw "'components.schemas' is not a valid PSCustomObject. Please verify the input JSON structure."
	}

	$uniqueModels = @{}
	$modelMap = @{}

	# Detect and map duplicate models
	foreach ($property in $schemas.PSObject.Properties) {
		$key = $property.Name
		$baseName = $key -replace '\d+$'
		if (-not $uniqueModels.ContainsKey($baseName)) {
			$uniqueModels[$baseName] = $key
		} else {
			if ($schemas.$($uniqueModels[$baseName]).GetHashCode() -eq $schemas.$key.GetHashCode()) {
				$modelMap[$key] = $uniqueModels[$baseName]
			}
		}
	}

	# Update all $ref values recursively
	Replace-RefsRecursively -Object $spec -ModelMap $modelMap

	# Remove duplicate models
	foreach ($key in $modelMap.Keys) {
		$schemas.PSObject.Properties.Remove($key)
	}

	# Fix invalid $ref entries
	Fix-Refs -node $spec -parent $null

	# Save the modified spec
	$spec | ConvertTo-Json -Depth 100 | Set-Content -Path $SpecFile
	Write-Host "Successfully updated the OpenAPI spec and saved to $SpecFile."
} catch {
	Write-Error "Error: $_"
}
