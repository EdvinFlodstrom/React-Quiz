import { useState } from 'react';
import GetQuestionByIdForm from './GetQuestionByIdForm';

const ModifyQuestion = ({ adjustGradient }) => {
    const [getQuestionByIdButtonDisabled, setGetQuestionByIdButtonDisabled] =
        useState(true);
    const [questionId, setQuestionId] = useState(0);

    const handleGetQuestionFormChange = (e) => {
        const { value } = e.target;

        const intValue = Number(value);

        setQuestionId(intValue);
        setGetQuestionByIdButtonDisabled(intValue <= 0);
    };

    const handleGetQuestion = (e) => {
        e.preventDefault();
    };

    return (
        <>
            <h1>Modify Question</h1>
            <h3>
                To modify a question, please follow the instructions below...
            </h3>

            <GetQuestionByIdForm
                handleGetQuestion={handleGetQuestion}
                questionId={questionId}
                handleGetQuestionFormChange={handleGetQuestionFormChange}
                getQuestionByIdButtonDisabled={getQuestionByIdButtonDisabled}
            />
        </>
    );
};

export default ModifyQuestion;
