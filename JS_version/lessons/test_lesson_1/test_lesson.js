module.exports = {
    Lesson:
    {
        Title: "test_lesson_1",
        Theory: [
            {
                Title: "info11",
                Text: "test_text_11",
                Pic: []
            },
            {
                Title: "info12",
                Text: "test_text_12",
                Pic: ["./lessons/test_lesson_1/pic/pic1.jpg"]
            },
        ],
        Tests: [
            {
                Title: "test11",
                Pic: [],
                Text: "test_test_11",
                Variants: [
                    "False1",
                    "False2",
                    "True",
                    "False3",                    
                ],
                Answer: 2
            },
            {
                Title: "test12",
                Pic: ["./lessons/test_lesson_1/pic/pic1.jpg"],
                Text: "test_test_12",
                Variants: [
                    "False1",
                    "False2",
                    "True",
                    "False3",                    
                ],
                Answer: 2
            },
        ]
    }
}