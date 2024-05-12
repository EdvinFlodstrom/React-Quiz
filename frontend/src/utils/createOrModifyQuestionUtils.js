export function formInputIsInvalid(updatedFormData) {
    if (
        !propertiesAreValid(
            updatedFormData.questionType,
            updatedFormData.question,
            updatedFormData.option1,
            updatedFormData.option2,
            updatedFormData.option3,
            updatedFormData.option4
        )
    )
        return true;

    if (
        updatedFormData.correctOptionNumber < 1 ||
        updatedFormData.correctOptionNumber > 4
    )
        return true;

    return false;
}

export function propertiesAreValid(...properties) {
    return properties.every(
        (property) =>
            property !== undefined && property !== null && property !== ''
    );
}
