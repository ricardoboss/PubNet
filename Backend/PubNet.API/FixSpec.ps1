param (
	[Parameter(Mandatory = $true)]
	[string]$SpecFile
)

function Replace-ObsoleteRefs {
	param (
		[PSCustomObject]$Object,
		[hashtable]$ModelMap
	)
	foreach ($property in $Object.PSObject.Properties) {
		if ($property.Value -is [PSCustomObject]) {
			# Recursively handle nested objects
			Replace-ObsoleteRefs -Object $property.Value -ModelMap $ModelMap
		} elseif ($property.Value -is [System.Collections.IEnumerable] -and -not ($property.Value -is [string])) {
			# Recursively handle arrays
			foreach ($item in $property.Value) {
				if ($item -is [PSCustomObject]) {
					Replace-ObsoleteRefs -Object $item -ModelMap $ModelMap
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

function Find-KeyCaseInsensitive {
	param(
		[PSCustomObject]$obj,
		[string]$key
	)
	$obj.PSObject.Properties | Where-Object { $_.Name -ieq $key } | Select-Object -First 1
}

# Recursive function to fix invalid $ref entries
function Fix-InvalidNestedRefs {
	param (
		[PSCustomObject]$root,
		[PSCustomObject]$node,
		[PSCustomObject]$parent
	)

	foreach ($key in $node.PSObject.Properties.Name) {
		$value = $node.$key

		if ($key -eq '$ref' -and $value -match '^#/components/schemas/#/.*') {
			# Split the ref path by '/' and drop everything before the last 'properties'
			$refParts = $value -split '/'
			$lastPropertiesIndex = [Array]::LastIndexOf($refParts, 'properties')

			if ($lastPropertiesIndex -gt 0) {
				# Extract the part before the last 'properties' to get the target schema
				$targetParts = $refParts[($lastPropertiesIndex - 1)..($refParts.Length - 1)]

				# Traverse the schema using the base path and relevant parts
				$targetSchema = $root.components.schemas

				# Traverse to the target schema based on the basePath
				foreach ($part in $targetParts) {
					$targetProperty = Find-KeyCaseInsensitive -obj $targetSchema -key $part
					if ($targetProperty) {
						$targetSchema = $targetProperty.Value

						if ($targetProperty.Name -eq '$ref') {
							break
						}
					}
					else
					{
						throw "Could not find '$part' of target schema '" + ($targetParts -join '/') + "' in available keys $( $targetSchema.PSObject.Properties.Name -join ', ' )"
					}
				}

				# If path is valid, replace the ref with the found schema
				$node.$key = $targetSchema.'$ref'
			}
		} elseif ($value -is [PSCustomObject]) {
			Fix-InvalidNestedRefs -root $root -node $value -parent $node
		} elseif ($value -is [System.Collections.IEnumerable]) {
			foreach ($item in $value) {
				if ($item -is [PSCustomObject]) {
					Fix-InvalidNestedRefs -root $root -node $item -parent $node
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
	Replace-ObsoleteRefs -Object $spec -ModelMap $modelMap

	# Remove duplicate models
	foreach ($key in $modelMap.Keys) {
		$schemas.PSObject.Properties.Remove($key)
	}

	# Fix invalid $ref entries
	Fix-InvalidNestedRefs -root $spec -node $spec -parent $null

	# Save the modified spec
	$spec | ConvertTo-Json -Depth 100 | Set-Content -Path $SpecFile
	Write-Host "Successfully updated the OpenAPI spec and saved to $SpecFile."
} catch {
	Write-Error "Error: $_"
}
