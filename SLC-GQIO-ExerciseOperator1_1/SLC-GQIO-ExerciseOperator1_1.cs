using Skyline.DataMiner.Analytics.GenericInterface;

/// <summary>
/// This class defines a GQI custom operator that takes:
/// 1. a column with values and
/// 2. a column with percentages (%) that the values present,
/// and calculates the total value for each row.
/// </summary>
[GQIMetaData(Name = "Derive total from percentage")]
public sealed class ExerciseOperator1 : IGQIColumnOperator, IGQIRowOperator, IGQIInputArguments
{
    // This is the column we want to add to the result and populate with the total values
    private readonly GQIColumn<double> _totalColumn = new GQIDoubleColumn("Total");



    // These are the arguments we that we'll need the query builder to provide:
    // A value representing a part of the total
    private readonly GQIArgument<GQIColumn> _valueColumnArgument = new GQIColumnDropdownArgument("Value")
    {
        IsRequired = true,
        Types = new[] { GQIColumnType.Double },
    };

    // A percentage that indicates how much the value presents of the total
    private readonly GQIArgument<GQIColumn> _percentColumnArgument = new GQIColumnDropdownArgument("Percentage (%)")
    {
        IsRequired = true,
        Types = new[] { GQIColumnType.Double },
    };



    // Here we'll store the values that GQI gives for our arguments
    private GQIColumn<double> _valueColumn;
    private GQIColumn<double> _percentColumn;



    // Here we can define which arguments our operator has
    public GQIArgument[] GetInputArguments()
    {
        return new GQIArgument[]
        {
            _valueColumnArgument,
            _percentColumnArgument,
        };
    }



    // Here we retrieve the values given for the arguments we defined
    public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
    {
        _valueColumn = (GQIColumn<double>)args.GetArgumentValue(_valueColumnArgument);
        _percentColumn = (GQIColumn<double>)args.GetArgumentValue(_valueColumnArgument);

        return default;
    }



    // Here we add the new column that will contain the total values
    public void HandleColumns(GQIEditableHeader header)
    {
        header.AddColumns(_totalColumn);
    }



    // Here we can retrieve the values from the selected columns and calculate the total
    public void HandleRow(GQIEditableRow row)
    {
        // Get the existing cell values
        double value = row.GetValue(_valueColumn);
        double percent = row.GetValue(_percentColumn);

        if (percent == 0)
        {
            // If the percentage is 0%, we cannot calculate the total :(
            // let's leave the cell empty
            return;
        }

        // Calculate the total value
        double total = 100 * value / percent;

        // Write the total to our new column
        row.SetValue(_totalColumn, value);
    }
}