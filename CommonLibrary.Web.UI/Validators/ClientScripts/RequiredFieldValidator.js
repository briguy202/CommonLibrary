function RequiredFieldValidatorIsValid(val) {
	var result = (ValidatorTrim(ValidatorGetValue(val.controltovalidate)) != ValidatorTrim(val.initialValue));
	return result;
}